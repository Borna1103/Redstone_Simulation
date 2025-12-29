using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redstone_Simulation.Models;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<Grid>(provider => new Grid(100, 100));

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Serve static files (your HTML, CSS, JS in wwwroot/)
app.UseStaticFiles();

// Optional: enable routing (required for controllers)
app.UseRouting();

app.UseCors();
// Map controller endpoints
app.MapControllers();
app.MapFallbackToFile("index.html");


// Run the app
app.Run();
