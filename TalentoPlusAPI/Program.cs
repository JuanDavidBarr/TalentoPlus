using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TalentoPlus.Data;
using TalentoPlus.Repositories.Interfaces;
using TalentoPlus.Repositories.Implementations;
using TalentoPlus.Services.Interfaces;
using TalentoPlus.Services.Implementations;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Build connection string from environment variables
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

// Add DbContext with the connection string
builder.Services.AddDbContext<PostgresqlContext>(options =>
    options.UseNpgsql(connectionString));

// JWT Configuration
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "DefaultSecretKey12345678901234567890";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "TalentoPlusAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "TalentoPlusClients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

// Register Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// Register Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Controllers
builder.Services.AddControllers();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TalentoPlus API",
        Version = "v1",
        Description = "API para gestión de empleados de TalentoPlus S.A.S.",
        Contact = new OpenApiContact
        {
            Name = "TalentoPlus",
            Email = "contact@talentoplus.com"
        }
    });

    // Configurar JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TalentoPlus API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "TalentoPlus API Documentation";
    });
}

app.UseHttpsRedirection();

// Importante: Authentication debe ir antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();