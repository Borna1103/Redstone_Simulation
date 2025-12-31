namespace Redstone_Simulation.Models.Interfaces
{
    public enum Shape
    {
        Idle,
        Straight,
        Corner,
        TJunction,
        Cross
    }

    public enum Orientation
    {
        North,
        East,
        South,
        West
    }

    public interface IObject
    {
        string Id { get; }
        int Strength { get; set; }
        Shape Shape { get; set; }
        Orientation? Facing { get; set; }
        public HashSet<Direction> Connections { get; set; }
        void Toggle()
        {
            // Default Implimentation
        }
        void SetConnections(HashSet<Direction> connections);
    }
}