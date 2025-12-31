using Redstone_Simulation.Models;
using Redstone_Simulation.Models.Interfaces;

namespace Redstone_Simulation.Helpers
{
    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction d) => d switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East  => Direction.West,
            Direction.West  => Direction.East,
            _ => d
        };

        public static Direction ToDirection(this Orientation o) => o switch
        {
            Orientation.North => Direction.North,
            Orientation.South => Direction.South,
            Orientation.East  => Direction.East,
            Orientation.West  => Direction.West,
            _ => Direction.North
        };
    }
}
