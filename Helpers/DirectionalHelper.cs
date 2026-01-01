using Redstone_Simulation.Models;

namespace Redstone_Simulation.Helpers
{
    public static class DirectionHelper
    {
        public static (int dx, int dy) Offset(Direction d) => d switch
        {
            Direction.North => (0, -1), // move up
            Direction.East  => (1, 0),  // move right
            Direction.South => (0, 1),  // move down
            Direction.West  => (-1, 0), // move left
            _ => (0, 0)
        };
    }
}
