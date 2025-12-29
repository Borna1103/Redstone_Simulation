using System.Runtime.Serialization;
using Redstone_Simulation.Models;
using Redstone_Simulation.Models.Interfaces;
namespace Redstone_Simulation.Models
{
    public class Redstone : IObject
    {

        public string Id => "Redstone";
        public int Strength { get; set; }
        public Shape Shape { get; set; }
        public Orientation? Facing { get; set; }

        public HashSet<Direction> Connections { get; set; }
        
        public Redstone()
        {
            Strength = 0;
            Connections = new HashSet<Direction>();
            Shape = Shape.Idle;
            Facing = null;
        }

        public void SetConnections(HashSet<Direction> connections)
        {
            Connections.Clear();
            Connections.UnionWith(connections);

            UpdateModel();
        }

        private void UpdateModel()
        {
            switch (Connections.Count)
            {
                case 0:
                    Shape = Shape.Idle;
                    break;

                case 1:
                    Shape = Shape.Straight;
                    Facing = Connections.Contains(Direction.North) || Connections.Contains(Direction.South)
                        ? Orientation.North
                        : Orientation.East;
                    break;

                case 2:
                    if ((Connections.Contains(Direction.North) && Connections.Contains(Direction.South)) ||
                        (Connections.Contains(Direction.East) && Connections.Contains(Direction.West)))
                    {
                        Shape = Shape.Straight;
                        Facing = Connections.Contains(Direction.North)
                            ? Orientation.North
                            : Orientation.East;
                    }
                    else
                    {
                        Shape = Shape.Corner;
                        Facing = DetermineCornerOrientation();
                    }
                    break;

                case 3:
                    Shape = Shape.TJunction;
                    Facing = DetermineTJunctionOrientation();
                    break;

                case 4:
                    Shape = Shape.Cross;
                    break;
            }
        }

        private Orientation DetermineCornerOrientation()
        {
            if (Connections.Contains(Direction.North) && Connections.Contains(Direction.East))
                return Orientation.East;
            if (Connections.Contains(Direction.East) && Connections.Contains(Direction.South))
                return Orientation.South;
            if (Connections.Contains(Direction.South) && Connections.Contains(Direction.West))
                return Orientation.West;
            return Orientation.North;
        }

        private Orientation DetermineTJunctionOrientation()
        {
            if (!Connections.Contains(Direction.North))
                return Orientation.South;
            if (!Connections.Contains(Direction.East))
                return Orientation.West;
            if (!Connections.Contains(Direction.South))
                return Orientation.North;
            return Orientation.East;
        }



        public void SetShape(Shape shape, Orientation? facing)
        {
            Shape = shape;
            Facing = facing;

        }

        public void UpdateStrength(int newStrength)
        {
            if (newStrength < 0 || newStrength > 15)
            {
                throw new ArgumentException("Strength must be between 0 and 15.");
            }

            if (newStrength > Strength)
            {
                Strength = newStrength;
            }
        }
    }
}