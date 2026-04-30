using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using InstrutorPro.API.Middlewares;
using InstrutorPro.Application.Interfaces.Repositories;
using InstrutorPro.Application.Interfaces.Services;
using InstrutorPro.Application.UseCases.Instrutores.CadastrarInstrutor;
using InstrutorPro.Infrastructure.GovApi;
using InstrutorPro.Infrastructure.Jobs;
using InstrutorPro.Infrastructure.Payment;
using InstrutorPro.Infrastructure.Persistence;
using InstrutorPro.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ─────────────────────────────────────────────
// Controllers & Swagger
// ─────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InstrutorPro API",
        Version = "v1",
        Description = "API para conectar instrutores de trânsito credenciados a alunos que buscam habilitação."
    });

    // Adiciona suporte a JWT no Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────
// JWT Authentication
// ─────────────────────────────────────────────
var jwtSecret = configuration["JwtSettings:Secret"] ?? "changeme-at-least-32-chars-long!!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ─────────────────────────────────────────────
// Entity Framework Core — PostgreSQL
// ─────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// ─────────────────────────────────────────────
// MediatR — CQRS
// ─────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CadastrarInstrutorCommand).Assembly));

// ─────────────────────────────────────────────
// Repositórios (Infrastructure)
// ─────────────────────────────────────────────
builder.Services.AddScoped<IInstrutorRepository, InstrutorRepository>();
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();

// ─────────────────────────────────────────────
// Serviços externos
// ─────────────────────────────────────────────
builder.Services.AddHttpClient<IGovApiService, GovApiService>(client =>
{
    client.BaseAddress = new Uri(configuration["GovApi:BaseUrl"] ?? "https://api.senatran.gov.br/v1/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IPaymentService, AsaasPaymentService>(client =>
{
    client.BaseAddress = new Uri(configuration["Asaas:BaseUrl"] ?? "https://api.asaas.com/v3/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ─────────────────────────────────────────────
// Hangfire — Jobs agendados
// ─────────────────────────────────────────────
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(
            configuration.GetConnectionString("DefaultConnection") ?? string.Empty)));

builder.Services.AddHangfireServer();
builder.Services.AddScoped<ValidacaoCredenciamentoJob>();

// ─────────────────────────────────────────────
// Redis — Cache e sessões
// ─────────────────────────────────────────────
var redisConnection = configuration.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(redisConnection))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(redisConnection));
}

// ─────────────────────────────────────────────
// CORS
// ─────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ─────────────────────────────────────────────
var app = builder.Build();
// ─────────────────────────────────────────────

// Middleware global de exceções (deve ser o primeiro)
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InstrutorPro API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Hangfire Dashboard (apenas em desenvolvimento)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}

// Registra o job semanal de validação de credenciamentos
// Executa toda segunda-feira às 03:00 (UTC)
RecurringJob.AddOrUpdate<ValidacaoCredenciamentoJob>(
    "validacao-credenciamento-semanal",
    job => job.ExecutarAsync(),
    "0 3 * * 1");

app.Run();

// Expose Program for integration tests
public partial class Program { }
