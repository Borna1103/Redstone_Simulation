using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.Models;
using Redstone_Simulation.Helpers;
public class Repeater: IObject
{
    public string Id => "Repeater";

    public Orientation? Facing { get; set; }
    public Shape Shape { get; set; }
    public HashSet<Direction> Connections { get; set; } 
    public int DelayTicks { get; set; } = 1; // 1–4 redstone ticks
    public int Strength { get; set; }

    public Direction InputSide => Facing!.Value.ToDirection().Opposite();
    public Direction OutputSide => Facing!.Value.ToDirection();


    private int? scheduledTick = null;
    private int pendingStrength = 0;

    public Repeater()
    {
        Strength = 0;
        Connections = new HashSet<Direction>();
        Shape = Shape.Idle;
        Facing = Orientation.East;
    }

    public void SetConnections(HashSet<Direction> connections)
    {
        Connections.Clear();
        Connections.Add(InputSide);
        Connections.Add(OutputSide);
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

    public void Toggle() { DelayTicks = (DelayTicks % 4) + 1; }

    public void Rotate()
    {
        Facing = Facing switch
        {
            Orientation.North => Orientation.East,
            Orientation.East => Orientation.South,
            Orientation.South => Orientation.West,
            _ => Orientation.North
        };
    }
}