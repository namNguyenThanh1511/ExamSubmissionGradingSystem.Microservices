using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =================== 🔐 AUTHENTICATION SETUP ===================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "iam-service",     // issuer của bạn (phải trùng trong token)
            ValidAudience = "api-gateway",   // audience của bạn
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

// =================== 🌐 YARP & SWAGGER ===================
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});




// =================== ⏱ RATE LIMITER ===================
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });
});

var app = builder.Build();

// =================== 🧭 MIDDLEWARE PIPELINE ===================
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        // Sửa host Swagger để là Gateway URL
        var serverUrl = $"{httpReq.Scheme}://{httpReq.Host.Value}";
        swaggerDoc.Servers = new List<OpenApiServer> { new() { Url = serverUrl } };
    });
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/iam/swagger/v1/swagger.json", "IAM Service v1");
    options.SwaggerEndpoint("/course/swagger/v1/swagger.json", "Course Service v1");
    options.RoutePrefix = string.Empty;
});

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
// ✅ Cho phép IAM service (login/register) bỏ qua auth
app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/iam"), subApp =>
{
    subApp.UseRouting();
    subApp.UseAuthentication();
    subApp.UseAuthorization();
    subApp.UseEndpoints(endpoints =>
    {
        endpoints.MapReverseProxy();
    });
});

// ✅ Còn lại thì yêu cầu JWT
app.MapWhen(ctx => !ctx.Request.Path.StartsWithSegments("/iam"), subApp =>
{
    subApp.UseRouting();
    subApp.UseAuthentication();
    subApp.UseAuthorization();
    subApp.UseEndpoints(endpoints =>
    {
        endpoints.MapReverseProxy().RequireAuthorization();
    });
});

app.Run();
