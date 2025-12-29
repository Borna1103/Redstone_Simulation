using Redstone_Simulation.Models;
namespace Redstone_Simulation.Services;

public class SimulationEngine
{

    public void Step(Grid grid, long tick)
    {
        grid.PropagateSignals();
    }
}
