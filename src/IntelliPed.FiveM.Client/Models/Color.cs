namespace IntelliPed.FiveM.Client.Models;

public record Color(int R, int G, int B, int A = 255)
{
    public static Color Red => new(255, 0, 0);
    public static Color Blue => new(0, 255, 0);
    public static Color Green => new(0, 0, 255);
    public static Color White => new(255, 255, 255);
    public static Color Black => new(0, 0, 0);
    public static Color Purple => new(194, 162, 218);
}