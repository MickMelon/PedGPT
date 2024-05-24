namespace IntelliPed.Messages.AgentStatus;

public record SetAgentStatusRequest
{
    public required bool IsThinking { get; init; }
}