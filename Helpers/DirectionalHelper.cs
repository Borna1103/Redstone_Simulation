using Redstone_Simulation.Models;

namespace Redstone_Simulation.Helpers
{
    public static class DirectionHelper
    {
        public static (int dx, int dy) Offset(Direction d) => d switch
        {
            Direction.North => (-1, 0),
            Direction.East  => (0, 1),
            Direction.South => (1, 0),
            Direction.West  => (0, -1),
            _ => (0, 0)
        };
    }
}
