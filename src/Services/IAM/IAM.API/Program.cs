
using IAM.API.Extensions;
using IAM.API.Middleware;
using IAM.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using StackExchange.Redis;


namespace IAM.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers().AddXmlSerializerFormatters();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigureSwaggerForAuthentication();
            builder.Services.ConfigureJWT(builder.Configuration);
            builder.Services.ConfigureGlobalException();

            // Configure logging (Console + Debug)
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var environment = builder.Environment.EnvironmentName;
            builder.Logging.AddConsole();

            // Database connection
            var connectionString = builder.Configuration.GetConnectionString("IAMDb");
            builder.Services.AddDbContext<IAMDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Redis connection
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
                return ConnectionMultiplexer.Connect(redisConnection);
            });

            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new ApiError { Message = e.ErrorMessage }))
                        .ToList();

                    var response = new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Dữ liệu không hợp lệ",
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            var app = builder.Build();

            // 🔥 LOG MÔI TRƯỜNG VÀ CONNECTION INFO
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("🚀 Application starting in {Environment} environment", environment);
            logger.LogInformation("📦 SQL Server connection string: {Connection}", connectionString);
            logger.LogInformation("🔗 Redis connection: {Redis}", builder.Configuration.GetConnectionString("Redis"));

            app.UseMiddleware<JwtBlacklistMiddleware>();

            // Apply pending migrations automatically
            if (app.Environment.IsEnvironment("Development") || app.Environment.IsEnvironment("Production"))
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<IAMDbContext>();

                    try
                    {
                        if (db.Database.GetPendingMigrations().Any())
                        {
                            logger.LogInformation("🛠 Applying pending migrations...");
                            db.Database.Migrate();
                            logger.LogInformation("✅ Database migrations applied successfully.");
                        }
                    }
                    catch (SqlException ex) when (ex.Number == 2714 || ex.Number == 1801)  // Duplicate object/DB exists
                    {
                        // Log & skip (table/DB đã có)
                        scope.ServiceProvider.GetRequiredService<ILogger<Program>>().LogWarning("Migration skipped: Object already exists. {Error}", ex.Message);
                    }
                }
            }

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    var serverUrl = $"{httpReq.Scheme}://{httpReq.Host.Value}/iam";
                    swaggerDoc.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
                    {
                        new() { Url = serverUrl }
                    };
                });
            });

            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseExceptionHandler();

            app.MapControllers();

            app.Run();
        }
    }
}
