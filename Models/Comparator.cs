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

        public Comparator()
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

        public int CalculateOutput(int SouthStrength, int EastStrength, int WestStrength)
        {
            if (SouthStrength < EastStrength || SouthStrength < WestStrength)
            {
                return 0;
            }
            return SouthStrength - Math.Max(EastStrength, WestStrength);
        }

        
    }
}