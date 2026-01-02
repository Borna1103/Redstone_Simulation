using Redstone_Simulation.Helpers;
using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.DTOs;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
            Cells = new IObject[cols, rows]; 
        }

        // x = column, y = row
        public void PlaceObject(int x, int y, IObject obj)
        {
            if (!InBounds(x, y))
                throw new ArgumentOutOfRangeException("Coordinates are out of grid bounds.");

            Cells[x, y] = obj;
            
            UpdateGrid(x, y);
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                var (dx, dy) = DirectionHelper.Offset(d);
                int nx = x + dx;
                int ny = y + dy;
                if (InBounds(nx, ny) && Cells[nx, ny] is Redstone)
                    UpdateGrid(nx, ny);
            }
        }

        public void RemoveObject(int x, int y)
        {
            if (!InBounds(x, y))
                throw new ArgumentOutOfRangeException("Coordinates are out of grid bounds.");

            Cells[x, y] = null;

            UpdateGrid(x, y);

            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                var (dx, dy) = DirectionHelper.Offset(d);
                int nx = x + dx;
                int ny = y + dy;

                if (InBounds(nx, ny) && Cells[nx, ny] is Redstone)
                    UpdateGrid(nx, ny);
            }
        }

        public void PropagateFrom(int x, int y, int strength)
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
                    IObject obj = Cells[nx, ny]!;
                    IObject prev = Cells[cx, cy]!;
                    if (obj == null) continue;

                    if ((obj is Block || obj is Lamp) && prev is Torch) continue;
                    if ((obj is Block || obj is Lamp) && prev is Redstone rs && rs.Strength > 0)
                    {
                        obj.Strength = 15;
                        continue;
                    }

                    if (obj is Repeater repeater)
                    {
                        // Only accept power from input side
                        if (d != repeater.InputSide)
                            continue;

                        repeater.ReceiveSignal(s, (int)Tick);
                        continue;
                    }

                    if (obj is Torch or Lever or RedstoneBlock or Comparator) continue;
                    if (prev is Block || prev is Lamp) continue;
                    if (prev is Comparator cmp && d != cmp.OutputSide) continue;

                    if (obj.Strength < s - 1)
                    {
                        obj.Strength = s - 1;
                        queue.Enqueue((nx, ny, s - 1));
                    }
                }
            }
        }

        public void AdvanceTick()
        {
            Tick++;

            // Reset non-power objects
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[c, r] == null) continue;
                    if (Cells[c, r] is not (Torch or Lever or RedstoneBlock))
                        Cells[c, r]!.Strength = 0;
                }
            }

            // Feed repeater inputs
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[c, r] is not Repeater rep) continue;

                    var (dx, dy) = DirectionHelper.Offset(rep.InputSide);
                    int nx = c + dx;
                    int ny = r + dy;

                    bool powered = InBounds(nx, ny)
                        && Cells[nx, ny]?.Strength > 0;

                    rep.ReceiveInput(powered, (int)Tick);
                }
            }


            // Tick repeaters
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[c, r] is Repeater repeater)
                        repeater.OnTick((int)Tick, this, c, r);
                }
            }

            // Propagate repeater outputs
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[c, r] is not Repeater rep) continue;
                    if (rep.Strength <= 0) continue;

                    var (dx, dy) = DirectionHelper.Offset(rep.OutputSide);
                    int nx = c + dx;
                    int ny = r + dy;

                    if (!InBounds(nx, ny)) continue;

                    var target = Cells[nx, ny];
                    if (target == null) continue;

                    target.Strength = Math.Max(target.Strength, rep.Strength);
                    PropagateFrom(nx, ny, rep.Strength);
                }
            }

            // Propagate power sources
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    var obj = Cells[c, r];
                    if (obj == null) continue;

                    if ((obj is Torch or Lever or RedstoneBlock) && obj.Strength > 0)
                    PropagateFrom(c, r, obj.Strength); 
                }
            }

            // Handle comparators
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[c, r] is not Comparator cmp) continue;

                    int rear = GetStrengthFrom(c, r, cmp.RearSide);
                    int left = GetStrengthFrom(c, r, cmp.LeftSide);
                    int right = GetStrengthFrom(c, r, cmp.RightSide);

                    cmp.Strength = cmp.ComputeOutput(rear, left, right);
                    if (cmp.Strength <= 0) continue;


                    
                    PropagateFrom(c, r, cmp.Strength);
                }
            }

            // Update torches
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[c, r] is Torch torch)
                        torch.Strength = IsAdjacentBlockPowered(c, r) ? 0 : 15;
                }
            }
        }

        public void Toggle(int x, int y)
        {
            var obj = Cells[x, y];
            if (obj == null) return;
            obj.Toggle();
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < Cols && y >= 0 && y < Rows;
        }

        private void UpdateGrid(int x, int y)
        {
            var rs = Cells[x, y];
            if (rs == null) return;

            var connections = new HashSet<Direction>();
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                var (dx, dy) = DirectionHelper.Offset(d);
                int nx = x + dx;
                int ny = y + dy;

                if (InBounds(nx, ny) && Cells[nx, ny] != null)
                    connections.Add(d);
            }
            rs.SetConnections(connections);
        }

        private bool IsAdjacentBlockPowered(int x, int y)
        {
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                var (dx, dy) = DirectionHelper.Offset(d);
                int nx = x + dx;
                int ny = y + dy;

                if (!InBounds(nx, ny)) continue;
                var obj = Cells[nx, ny];
                if (obj == null) continue;
                if (obj is Block or Lamp && obj.Strength > 0)
                    return true;
            }
            return false;
        }

        private int GetStrengthFrom(int x, int y, Direction d)
        {
            var (dx, dy) = DirectionHelper.Offset(d);
            int nx = x + dx;
            int ny = y + dy;

            if (!InBounds(nx, ny) || Cells[nx, ny] == null)
                return 0;

            return Cells[nx, ny]!.Strength;
        }
    }
}
