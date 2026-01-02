namespace Redstone_Simulation.DTOs;

public record CellUpdate(
    int X,
    int Y,
    string? Type,
    string? Shape,
    string? Orientation,
    int? Strength,
    string? Mode
);
