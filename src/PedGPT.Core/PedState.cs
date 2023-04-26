namespace PedGPT.Core;

public class PedState
{
    public int Health { get; set; }
    public List<string> Weapons { get; set; } = new();
    public Vehicle? Vehicle { get; set; }
}