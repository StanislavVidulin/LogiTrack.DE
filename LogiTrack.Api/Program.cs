using LogiTrack.Business.Services;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Application.Interfaces;
using LogiTrack.Infrastructure.Data;
using LogiTrack.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();

// Add Swagger/OpenAPI support
// EndpointsApiExplorer is required for discovering controller routes
builder.Services.AddEndpointsApiExplorer();
// SwaggerGen handles the generation of the OpenAPI specification
builder.Services.AddSwaggerGen();

// Database configuration SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=logitrack.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database creation error");
    }
}

// 2. Configure the HTTP request pipeline.
// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();
// Enable middleware to serve Swagger UI (HTML, JS, CSS, etc.)
// This provides a visual interface for testing the API.
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

// 3. Map controller routes
app.MapControllers();

app.Run();