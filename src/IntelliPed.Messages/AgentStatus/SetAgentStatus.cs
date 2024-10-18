namespace IntelliPed.Messages.AgentStatus;

public record SetAgentStatusRequest
{
    public required EnAgentStatus Status { get; init; }
}

public enum EnAgentStatus
{
    None,
    Thinking,
    Acting,
    Observing,
}