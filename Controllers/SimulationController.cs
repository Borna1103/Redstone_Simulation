using Microsoft.AspNetCore.Mvc;
using Redstone_Simulation.Models;
using Redstone_Simulation.Models.Interfaces;
using Redstone_Simulation.Helpers;
using Redstone_Simulation.DTOs;

namespace Redstone_Simulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private static Grid _grid = new Grid(100, 100);

        [HttpPost("state")]
        public IActionResult GetState()
        {
            return Ok(CurrentGridState());
        }

        [HttpPost("place")]
        public IActionResult PlaceObject([FromBody] PlaceRequest req)
        {

            IObject obj = req.Type switch
            {
                "Redstone" => new Redstone(),
                "Torch" => new Torch(),
                "Lever" => new Lever(),
                "RedstoneBlock" => new RedstoneBlock(),
                "Block" => new Block(),
                "Repeater" => new Repeater(),
                "Comparator" => new Comparator(),
                "Lamp" => new Lamp(),
                _ => null
            };

            if (obj == null)
            {
                return BadRequest("Invalid object type.");
            }

            try{
                _grid.PlaceObject(req.X, req.Y, obj);         
                return Ok(CurrentGridState());
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("remove")]
        public IActionResult RemoveObject([FromBody] RemoveRequest req)
        {
            try
            {
                _grid.RemoveObject(req.X, req.Y);
                var updates = CurrentGridState();
                updates.Add(new CellUpdate(req.X, req.Y, null, null, null, null, null));
                return Ok(updates);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("clear")]
        public IActionResult ClearGrid()
        {
            
            var currGrid = CurrentGridState();
            for (int i = 0; i < currGrid.Count; i++)
            {
                currGrid[i] = new CellUpdate(currGrid[i].X, currGrid[i].Y, null, null, null, null, null);
            }
            _grid = new Grid(100, 100);
            return Ok(currGrid);
        }

        [HttpPost("tick")]
        public IActionResult Tick()
        {
            _grid.AdvanceTick();
            return Ok(CurrentGridState());
        }

        [HttpPost("toggle")]
        public IActionResult Toggle([FromBody] ToggleRequest req)
        {
            try
            {
                _grid.Toggle(req.X, req.Y);
                return Ok();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        public List<CellUpdate> CurrentGridState()
        {
            var cells = new List<CellUpdate>();

            for (int r = 0; r < _grid.Rows; r++)
            {
                for (int c = 0; c < _grid.Cols; c++)
                {
                    var obj = _grid.Cells[c, r];
                    if (obj == null) continue;

                    cells.Add(new CellUpdate(
                        c,
                        r,
                        obj.Id,
                        obj.Shape.ToString(),
                        obj.Facing?.ToString(),
                        obj.Strength,
                        obj.Mode?.ToString()
                    ));       
                }
            }
            return cells;
        }

        
    public record ToggleRequest(int X, int Y);
    public record PlaceRequest(int X, int Y, string Type);
    public record RemoveRequest(int X, int Y);

    }
}