using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Submission.Repositories;
using Submission.Repositories.Repositories;
using Submission.Services.DTOs;
using Submission.Services.StorageService;
using Submission.Services.StudentSubmissionService;
using Submission.Services.UploadService;

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
            builder.Services.AddSwaggerGen();

            // Sử dụng DI với IOptions<AwsConfig> trong AwsS3StorageService
            builder.Services.AddScoped<IStorageService, AwsS3StorageService>();
            builder.Services.AddScoped<IStudentSubmissionRepository, StudentSubmissionRepository>();
            builder.Services.AddScoped<IStudentSubmissionService, StudentSubmissionService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowAll",
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin()
                                             .AllowAnyMethod()
                                             .AllowAnyHeader();
                                  });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseCors("AllowAll");
            app.MapControllers();

            app.Run();
        }
    }
}
