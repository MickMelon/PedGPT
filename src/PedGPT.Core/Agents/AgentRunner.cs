using Microsoft.Extensions.Logging;

namespace PedGPT.Core.Agents;

public class AgentRunner
{
    public bool IsRunning { get; private set; }

    private readonly Agent _agent;
    private readonly ILogger<AgentRunner> _logger;

    public AgentRunner(Agent agent, ILogger<AgentRunner> logger)
    {
        _agent = agent;
        _logger = logger;
    }

    public async Task Run()
    {
        if (IsRunning)
            throw new InvalidOperationException("Agent is already running.");

        IsRunning = true;

        _logger.LogInformation("Agent running.");

        while (IsRunning)
        {
            try
            {
                var thinkResult = await _agent.Think();

                var actResult = await _agent.Act(thinkResult.Command?.Name ?? "none", thinkResult.Command?.Args ?? new());

                _agent.Observe(thinkResult, actResult);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Agent loop failed.");
            }

            await Task.Delay(10000);
        }
    }
}