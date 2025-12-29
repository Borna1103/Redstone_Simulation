using Redstone_Simulation.Models;

namespace Redstone_Simulation.Services;

public class SimulationService
{
    public Grid Grid { get; } = new(100,100);
    public long CurrentTick { get; private set; }

    private readonly SimulationEngine _engine;

    public SimulationService(SimulationEngine engine)
    {
        _engine = engine;
    }

    public void Tick()
    {
        CurrentTick++;
        _engine.Step(Grid, CurrentTick);
    }
}
