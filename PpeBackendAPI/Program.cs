using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PpeBackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using PpeBackendAPI.Services;



var builder = WebApplication.CreateBuilder(args);
var chave = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Chave JWT não configurada.");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));

builder.Services.AddDbContext<PpeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10_000_000; // 10MB
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("usuario", policy =>
        policy.RequireRole("usuario"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorCors", policy =>
    {
        policy.WithOrigins("http://localhost:5271",
                            "http://localhost:5239",
                            "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});




var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors("BlazorCors");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
