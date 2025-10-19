using System.Security.Claims;
using DockQueue.Infra.Ioc;
using Microsoft.OpenApi.Models;
using DockQueue.Domain.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------
// IoC
// -----------------------
builder.Services.AddInfrastructure(builder.Configuration);

// -----------------------
// Controllers
// -----------------------
builder.Services.AddControllers();

// -----------------------
// CORS (Blazor / Frontend)
// Configure no appsettings: "Cors:AllowedOrigins": ["https://localhost:5173", ...]
// -----------------------
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        else
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
    });
});

// -----------------------
// JWT Auth
// Usa AccessTokenMinutes / RefreshTokenDays do appsettings
// -----------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new Exception("JWT Key não configurada"));
var signingKey = new SymmetricSecurityKey(keyBytes);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Em DEV você pode desligar para testes em http
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // tokens expiram exatamente no horário
            // (opcional) mapeia role/email se quiser depender desses nomes
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Email
        };
    });

builder.Services.AddAuthorization();

// -----------------------
// Swagger centralizado no IoC
// -----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddInfrastructureSwagger();


var app = builder.Build();

// -----------------------
// Pipeline
// -----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        if (exception is DomainExceptionValidation)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { error = exception.Message }));
        }
    });
});

// Se for Blazor Server dentro do mesmo projeto, você pode precisar de:
// app.UseStaticFiles();

app.UseHttpsRedirection();

// CORS antes de auth/authorization
app.UseCors("Frontend");

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
