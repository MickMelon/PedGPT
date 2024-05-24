namespace IntelliPed.FiveM.Messages;

public record SpeakRequest
{
    public required string Message { get; init; }
}