using System.Runtime.Serialization;
using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.Models;
using System.Runtime.CompilerServices;
namespace Redstone_Simulation.Models
{
    public class Torch : IObject
    {
        public string Id => "Torch";
        public int Strength { get; set; }
        public Shape Shape { get; set; }
        public Orientation? Facing { get; set; }
        public string? Mode => null;
        public HashSet<Direction> Connections { get; set; }

        public Torch()
        {
            Strength = 15;
            Shape = Shape.Idle;
            Facing = null;
            Connections = new HashSet<Direction>();
        }

        public void SetConnections(HashSet<Direction> connections)
        {
            Connections.Clear();
            Connections.UnionWith(connections);
        }

    }
}