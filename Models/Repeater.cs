using System.Runtime.Serialization;
using Redstone_Simulation.Models;
using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.Helpers;

public class Repeater : IObject
{
    public string Id => "Repeater";
    public Orientation? Facing { get; set; }
    public Shape Shape { get; set; }
    public HashSet<Direction> Connections { get; set; } 

    public int DelayTicks { get; set; } = 1;
    public string Mode => DelayTicks.ToString();
    public int Strength { get; set; }

    private int scheduledTick = -1;
    private bool poweredInput = false;

    public Direction OutputSide =>
        DirectionExtensions.ToDirection(Facing!.Value);

    public Direction InputSide =>
        DirectionExtensions.Opposite(OutputSide);

    public Repeater()
        {
            Strength = 0;
            Shape = Shape.Idle;
            Facing = Orientation.East;
            Connections = new HashSet<Direction>();
        }
    public void SetConnections(HashSet<Direction> connections)
    {
        Connections.Clear();
        Connections.Add(OutputSide);
    }

    public void ReceiveInput(bool powered, int currentTick)
    {
        if (powered == poweredInput) return;

        poweredInput = powered;
        scheduledTick = currentTick + DelayTicks;
    }
    public void ReceiveSignal(int strength, int currentTick)
    {
        ReceiveInput(strength > 0, currentTick);
    }


    public void OnTick(int currentTick, Grid grid, int x, int y)
    {
        if (scheduledTick == -1 || currentTick < scheduledTick) return;

        // Set the repeater output strength
        Strength = poweredInput ? 15 : 0;
        scheduledTick = -1;

        // Only propagate if powered
        var (dx, dy) = DirectionHelper.Offset(OutputSide);
        int nx = x + dx;
        int ny = y + dy;

        if (!grid.InBounds(nx, ny)) return;

        var target = grid.Cells[nx, ny];
        if (target == null) return;

        // Set neighbor strength if it would increase it
        if (target.Strength < Strength)
            target.Strength = Strength;

        // Propagate the signal
        grid.PropagateFrom(nx, ny, Strength);
    }

    

    public void Toggle()
    {
        DelayTicks = (DelayTicks % 4) + 1;
    }

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
