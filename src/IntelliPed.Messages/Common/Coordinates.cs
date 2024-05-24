namespace IntelliPed.Messages.Common;

public record Coordinates
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Coordinates(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}