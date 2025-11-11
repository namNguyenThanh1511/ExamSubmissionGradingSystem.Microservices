
using Microsoft.OpenApi.Models;

namespace CourseManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {

                        // Swagger UI gọi qua Gateway nên cần host là Gateway
                        //var serverUrl = $"{httpReq.Scheme}://{httpReq.Host.Value}/iam";
                        var serverUrl = "http://localhost:5103/course"; // Gateway host + path /iam
                        swaggerDoc.Servers = new List<OpenApiServer>
                        {
                            new() { Url = serverUrl }
                        };
                    });
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            // Sau app.MapControllers();
            app.MapGet("/health", () => Results.Ok(" API is healthy!"));
            app.Run();
        }
    }
}
