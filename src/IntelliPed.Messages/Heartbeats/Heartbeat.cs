using IntelliPed.Messages.Common;

namespace IntelliPed.Messages.Heartbeats;

public record Heartbeat
{
    public required Coordinates Coordinates { get; init; }

    public override string ToString()
    {
        return $"Coordinates: {Coordinates}";
    }
}