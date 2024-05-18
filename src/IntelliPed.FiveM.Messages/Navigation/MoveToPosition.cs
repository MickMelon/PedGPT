namespace IntelliPed.FiveM.Messages.Navigation;

public record MoveToPositionRequest
{
    public float X { get; set; }    
    public float Y { get; set; }    
    public float Z { get; set; }
}