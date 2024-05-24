namespace IntelliPed.Messages.Signals;

public record SpeechSignal : Signal
{
    public required string Message { get; init; }

    public override string ToString()
    {
        return $"You heard someone say, \"{Message}\"";
    }
}