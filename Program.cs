using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redstone_Simulation.Models;
using Redstone_Simulation.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<Grid>(provider => new Grid(100, 100));

builder.Services.AddSingleton<SimulationEngine>();
builder.Services.AddSingleton<SimulationService>();
builder.Services.AddHostedService<SimulationTickService>();
builder.Services.AddControllers();

var app = builder.Build();

// Serve static files (your HTML, CSS, JS in wwwroot/)
app.UseStaticFiles();

// Optional: enable routing (required for controllers)
app.UseRouting();

// Map controller endpoints
app.MapControllers();
app.MapFallbackToFile("index.html");


// Run the app
app.Run();
