namespace IntelliPed.Messages.Speech;

public record SpeakRequest
{
    public required string Message { get; init; }
}