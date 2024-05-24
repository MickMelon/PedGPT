namespace IntelliPed.FiveM.Messages.Sensors;

public record SpeechHeardEvent
{
    public required string Message { get; init; }
}