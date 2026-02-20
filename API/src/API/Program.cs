using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using PersonalFinanceAPI.Infrastructure.DependencyInjection;
using PersonalFinanceAPI.API.Middleware;
using PersonalFinanceAPI.API.Endpoints;
using PersonalFinanceAPI.Application.Features.Transactions;
using PersonalFinanceAPI.Application.Features.Auth.Commands;
using FluentValidation;
using Scalar.AspNetCore;
using PersonalFinanceAPI.Application.Features.Auth;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not found in configuration");

var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PersonalFinanceAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PersonalFinanceAPI-Clients";

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new { error = "Authentication failed" });
            }
        };
    });

// Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddDefaultPolicy("RequireAuthentication", policy => policy.RequireAuthenticatedUser());

// Services
builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
        typeof(CreateTransactionCommand).Assembly,
        typeof(RegisterCommand).Assembly))
    .AddValidatorsFromAssemblyContaining<CreateTransactionCommand>()
    .AddInfrastructure(connectionString);

// CORS - Restrito ao domínio Angular
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(",") ?? ["http://localhost:4200"];
    
    options.AddPolicy("AllowAngularApp", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Apply migrations
await app.Services.ApplyMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Personal Finance API")
            .WithTheme(ScalarTheme.DeepSpace);
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Map endpoints
app.MapAuthEndpoints();
app.MapTransactionEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("Health Check")
    .AllowAnonymous();

app.Run();
