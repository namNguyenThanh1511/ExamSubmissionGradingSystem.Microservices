
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
                        var serverUrl = $"{httpReq.Scheme}://{httpReq.Host.Value}/course";
                        swaggerDoc.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
                        {
                            new() { Url = serverUrl }
                        };
                    });
                });

                app.UseSwaggerUI();
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
