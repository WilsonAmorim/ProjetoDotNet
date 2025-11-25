using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PpeBackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using PpeBackendAPI.Services;
// A linha 'using MySql.Data.MySqlClient;' foi removida para resolver o erro CS0246, 
// pois o provedor EF Core já deve estar configurado corretamente.

var builder = WebApplication.CreateBuilder(args);
var chave = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Chave JWT não configurada.");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var env = builder.Environment.EnvironmentName;

builder.Services.AddDbContext<PpeDbContext>(options =>
{
    if (env == "Development")
    {
        // Usa SQLite no ambiente de desenvolvimento
        options.UseSqlite(connectionString);
    }
    else
    {
        // Usa MariaDB em produção
        // O método ServerVersion.AutoDetect requer o using correto do provedor MySql
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        );
    }
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10_000_000; // 10MB
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Autenticação (Quem é o usuário)
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

// Autorização (O que o usuário pode fazer)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("usuario", policy =>
        policy.RequireRole("usuario"));
});

// CORS (INCLUINDO A ORIGEM DO SEU FRONT-END NO IIS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5271",
                "http://localhost:5239",
                "http://localhost:5173",
                // *** CORREÇÃO CRUCIAL PARA O IIS: Adicionando o Front-end Blazor ***
                "http://ppeprojeto.saeb:5001"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// --- CORREÇÃO DA ORDEM DO MIDDLEWARE (NECESSÁRIO PARA USEAUTHORIZATION) ---

// 1. CORS DEVE VIR ANTES de Autenticação/Autorização
app.UseCors("BlazorCors");

// 2. Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// 3. Mapeamento de Controllers (O roteamento)
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ... o restante do código MapGet e app.Run()...

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}