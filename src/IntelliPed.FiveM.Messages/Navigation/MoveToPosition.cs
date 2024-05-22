namespace IntelliPed.FiveM.Messages.Navigation;

public record MoveToPositionRequest
{
    public required float X { get; init; }    
    public required float Y { get; init; }    
    public required float Z { get; init; }
}