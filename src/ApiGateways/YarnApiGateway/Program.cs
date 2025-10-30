using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
// Swagger UI Aggregator
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });
});

var app = builder.Build();
// Swagger UI tổng hợp 👇
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/iam/swagger/v1/swagger.json", "IAM Service v1");
    options.SwaggerEndpoint("/course/swagger/v1/swagger.json", "Course Service v1");
    options.RoutePrefix = string.Empty;
});


// Configure the HTTP request pipeline.
app.UseRateLimiter();

app.MapReverseProxy();

app.Run();
