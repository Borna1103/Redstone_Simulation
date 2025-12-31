using System.Runtime.Serialization;
using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.Models;
namespace Redstone_Simulation.Models
{
    public class Lever : IObject
    {
        public string Id => "Lever";
        public int Strength { get; set; }
        public Shape Shape { get; set; }
        public Orientation? Facing { get; set; }

        public HashSet<Direction> Connections { get; set; }

        public Lever()
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

        public void Toggle()
        {
            if (Strength > 0)
            {
                Strength = 0;
            }
            else
            {
                Strength = 15;
            }
        }
    }
}