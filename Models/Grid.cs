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
            

            if (obj is Redstone rs)
            {
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

            else if (obj is Torch)
            {
                PowerObjects(x,y, Strength: obj.Strength);
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
            PropagateSignals();
        }

        public void PowerObjects(int x, int y, int Strength)
        {
            var updates = new List<CellUpdate>();
            while (Strength > 0)
            {
                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    var (dx, dy) = DirectionHelper.Offset(d);
                    int nx = x + dx;
                    int ny = y + dy;

                    if (InBounds(nx, ny) && Cells[nx, ny] is IObject neighbor)
                    {
                        if (neighbor.Strength < Strength - 1)
                        {
                            neighbor.Strength = Strength - 1;
                            PowerObjects(nx, ny, neighbor.Strength);
                        }
                    }
                }
                break; // Prevent infinite loop
            }
            UpdateGrid(x, y);
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
            rs.Strength = highestStrength-1;
            rs.SetConnections(connections);
            
        }

        

        public void PropagateSignals()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[r, c] is not Torch) 
                    {
                        if (Cells[r, c] == null) continue;  
                        Cells[r, c]!.Strength = 0;
                    }
                    
                        
                }
            }

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[r, c] is Torch torch)
                    {
                        PowerObjects(r, c, torch.Strength);
                    }
                }
            }
        }

        
    }
}