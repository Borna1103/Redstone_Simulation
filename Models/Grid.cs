using Redstone_Simulation.Helpers;
using Redstone_Simulation.Models;
using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.DTOs;
namespace Redstone_Simulation.Models
{
    public class Grid
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public IObject?[,] Cells { get; set; }
        public long Tick { get; set; } = 0;

        public Grid(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Cells = new IObject[rows, cols];
        }

        public void PlaceObject(int x, int y, IObject obj)
        {
            if (!InBounds(x,y))
            {
                throw new ArgumentOutOfRangeException("Coordinates are out of grid bounds.");
            }

            Cells[x, y] = obj;
            
            UpdateGrid(x, y);
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                var (dx, dy) = DirectionHelper.Offset(d);
                int nx = x + dx;
                int ny = y + dy;
                if (InBounds(nx, ny) && Cells[nx, ny] is Redstone neighbor)
                { 
                    UpdateGrid(nx, ny);
                }
            }
        }

        public void RemoveObject(int x, int y)
        {
            if (!InBounds(x,y))
            {
                throw new ArgumentOutOfRangeException("Coordinates are out of grid bounds.");
            }

            Cells[x, y] = null;
            
            UpdateGrid(x, y);

            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                var (dx, dy) = DirectionHelper.Offset(d);
                int nx = x + dx;
                int ny = y + dy;

                if (InBounds(nx, ny) && Cells[nx, ny] is Redstone neighbor)
                { 
                    UpdateGrid(nx, ny);
                }
            }
        }

        private void PropagateFrom(int x, int y, int strength)
        {
            var queue = new Queue<(int x, int y, int strength)>();
            queue.Enqueue((x, y, strength));

            while (queue.Count > 0)
            {
                var (cx, cy, s) = queue.Dequeue();
                if (s <= 0) continue;

                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    var (dx, dy) = DirectionHelper.Offset(d);
                    int nx = cx + dx;
                    int ny = cy + dy;

                    if (!InBounds(nx, ny)) continue;
                    if (Cells[nx, ny] is not Redstone rs) continue;

                    if (rs.Strength < s - 1)
                    {
                        rs.Strength = s - 1;
                        queue.Enqueue((nx, ny, s - 1));
                    }
                }
            }
        }

        public void AdvanceTick()
        {
            Tick++;

            // Reset non-torch strengths
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[r, c] is Redstone rs)
                        rs.Strength = 0;
                }
            }

            // Power from sources
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[r, c] is Torch)
                        PropagateFrom(r, c, 15);
                }
            }
        }


        

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < Rows && y >= 0 && y < Cols;
        }

        private void UpdateGrid(int x, int y)
        {
            var rs = Cells[x, y] as IObject;
            if (rs == null) return;

            var connections = new HashSet<Direction>();
            var highestStrength = 0;
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                var (dx, dy) = DirectionHelper.Offset(d);
                int nx = x + dx;
                int ny = y + dy;

                if (InBounds(nx, ny) && Cells[nx, ny] is IObject)
                {
                    highestStrength = Math.Max(highestStrength, Cells[nx, ny]!.Strength);
                    connections.Add(d);
                }
            }
            rs.SetConnections(connections);
        }

        
    }
}