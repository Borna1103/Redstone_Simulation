using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.Models;
public class Repeater: IObject, ISimulatable
{
    public string Id => "Repeater";

    public Orientation? Facing { get; set; }
    public Shape Shape { get; set; }
    public HashSet<Direction> Connections { get; set; } 
    public int DelayTicks { get; set; } = 2; // 1–4 redstone ticks
    public int Strength { get; set; }

    private int? scheduledTick = null;
    private int pendingStrength = 0;

    public Repeater()
    {
        Strength = 0;
        Connections = new HashSet<Direction>();
        Shape = Shape.Straight;
        Facing = Orientation.North;
    }

    public void SetConnections(HashSet<Direction> connections)
    {
        Connections.Clear();
        Connections.UnionWith(connections);
    }

    public void ReceiveSignal(int inputStrength, int currentTick)
    {
        pendingStrength = inputStrength;
        scheduledTick = currentTick + DelayTicks;
    }


    public void OnTick(int currentTick)
    {
        if (scheduledTick.HasValue && currentTick >= scheduledTick.Value)
        {
            Strength = pendingStrength;
            scheduledTick = null;
        }
    }

    private bool CheckInputPower()
    {
        // Logic to check if the input side is powered
        // This is a placeholder and should be replaced with actual grid checking logic
        return false;
    }
}