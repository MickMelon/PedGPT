namespace PedGPT.Core;

public class WorldState
{
    public List<Location> Locations { get; set; } = new();
    public List<Vehicle> Vehicles { get; set; } = new();
    public List<Ped> Peds { get; set; } = new();
}