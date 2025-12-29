using Microsoft.Extensions.Hosting;

namespace Redstone_Simulation.Services;

public class SimulationTickService : BackgroundService
{
    private readonly SimulationService _simulation;

    public SimulationTickService(SimulationService simulation)
    {
        _simulation = simulation;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int tickMs = 100;

        while (!stoppingToken.IsCancellationRequested)
        {
            _simulation.Tick();
            await Task.Delay(tickMs, stoppingToken);
        }
    }
}
