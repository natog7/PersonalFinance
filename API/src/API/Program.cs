using MediatR;
using PersonalFinanceAPI.Infrastructure.DependencyInjection;
using PersonalFinanceAPI.API.Middleware;
using PersonalFinanceAPI.API.Endpoints;
using PersonalFinanceAPI.Application.Features.Transactions;
using FluentValidation;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not found in configuration");

// Add services
builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateTransactionCommand).Assembly))
    .AddValidatorsFromAssemblyContaining<CreateTransactionCommand>()
    .AddInfrastructure(connectionString);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Apply migrations
await app.Services.ApplyMigrationsAsync();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
	// Add Scalar API reference for documentation
	app.MapScalarApiReference(options =>
    {
        options.WithTitle("Personal Finance API")
        .WithTheme(ScalarTheme.DeepSpace);
	});
}

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Map endpoints
app.MapTransactionEndpoints();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("Health Check");

app.Run();
