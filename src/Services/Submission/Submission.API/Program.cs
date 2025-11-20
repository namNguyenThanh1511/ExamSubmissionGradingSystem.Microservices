using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Submission.Repositories;
using Submission.Repositories.Repositories;
using Submission.Services.DTOs;
using Submission.Services.StorageService;
using Submission.Services.StudentSubmissionService;
using Submission.Services.UploadService;
using Swashbuckle.AspNetCore.Filters;

namespace Submission.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load .env trước khi bind configuration
            Env.Load();
            builder.Configuration.AddEnvironmentVariables();

            // Bind AwsConfig và đăng ký vào DI
            builder.Services.Configure<AwsConfig>(builder.Configuration.GetSection("AWS"));

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SubmissionDB")));
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                        "Example: \"Bearer [token]\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                });
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Submission API", Version = "v1" });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            // Sử dụng DI với IOptions<AwsConfig> trong AwsS3StorageService
            builder.Services.AddScoped<IStorageService, AwsS3StorageService>();
            builder.Services.AddScoped<IStudentSubmissionRepository, StudentSubmissionRepository>();
            builder.Services.AddScoped<IStudentSubmissionService, StudentSubmissionService>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.SetIsOriginAllowed(origin => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    // Support both direct API access and Gateway access
                    // Direct API as default (first in list)
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}", Description = "Direct API Access (Use this for testing)" },
                        new OpenApiServer { Url = "http://localhost:5103/submission", Description = "Via API Gateway (Production)" }
                    };
                });
            });
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseRouting();                                     // 1. Routing first
            app.UseCors();                                        // 2. CORS after routing
            app.UseAuthentication();                              // 4. Authentication
            app.UseAuthorization();                               // 5. Authorization

            app.MapControllers();

            app.Run();
        }
    }
}
