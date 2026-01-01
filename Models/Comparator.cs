using System.Runtime.Serialization;
using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.Models;
namespace Redstone_Simulation.Models
{
    public class Comparator : IObject
    {
        public string Id => "Comparator";
        public int Strength { get; set; }
        public Shape Shape { get; set; }
        public Orientation? Facing { get; set; }

        public HashSet<Direction> Connections { get; set; }
        public bool Mode = false;

        public Direction OutputSide => Facing switch
        {
            Orientation.North => Direction.North,
            Orientation.South => Direction.South,
            Orientation.East  => Direction.East,
            Orientation.West  => Direction.West,
            _ => Direction.East
        };

        public Direction RearSide => OutputSide switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East  => Direction.West,
            Direction.West  => Direction.East,
            _ => Direction.West
        };

        public Direction LeftSide => OutputSide switch
        {
            Direction.North => Direction.West,
            Direction.South => Direction.East,
            Direction.East  => Direction.North,
            Direction.West  => Direction.South,
            _ => Direction.North
        };

        public Direction RightSide => OutputSide switch
        {
            Direction.North => Direction.East,
            Direction.South => Direction.West,
            Direction.East  => Direction.South,
            Direction.West  => Direction.North,
            _ => Direction.South
        };


        public Comparator()
        {
            Strength = 0;
            Shape = Shape.Idle;
            Facing = Orientation.East;
            Connections = new HashSet<Direction>();
        }

        public void SetConnections(HashSet<Direction> connections)
        {
            Connections.Clear();
            Connections.UnionWith(connections);
        }

        public int ComputeOutput(int rear, int left, int right)
        {
            if (!Mode) return Compare(rear, left, right);
            return Subtract(rear, left, right);
        }

        private int Compare(int rear, int left, int right)
        {
            return (rear >= Math.Max(left, right)) ? rear : 0;
        }

        private int Subtract(int rear, int left, int right)
        {
            return Math.Max(rear - Math.Max(left, right), 0);
        }

        public void Toggle()
        {
            Mode = !Mode;
        }

        
    }
}