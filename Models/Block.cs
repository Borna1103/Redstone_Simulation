using System.Runtime.Serialization;
using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.Models;
namespace Redstone_Simulation.Models
{
    public class Block : IObject
    {
        public string Id => "Block";
        public int Strength { get; set; }
        public Shape Shape { get; set; }
        public Orientation? Facing { get; set; }

        public HashSet<Direction> Connections { get; set; }

        public Block()
        {
            Strength = 0;
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