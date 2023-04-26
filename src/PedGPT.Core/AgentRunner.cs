using System.Text.Json;
using Microsoft.Extensions.Logging;
using PedGPT.Core.Actions;

namespace PedGPT.Core;

public class AgentRunner
{
    public bool IsRunning { get; private set; }

    private readonly Agent _agent;
    private readonly ActionLocator _actionLocator;
    private readonly ILogger<AgentRunner> _logger;

    public AgentRunner(
        Agent agent, 
        ActionLocator actionLocator, 
        ILogger<AgentRunner> logger)
    {
        _agent = agent;
        _actionLocator = actionLocator;
        _logger = logger;
    }

    /// <summary>
    /// Executes the TAO loop for the autonomous pedestrian. This method
    /// iteratively calls the `Think`, `Act`, and `Observe` methods
    /// of the pedestrian, generating plans and strategies, executing actions,
    /// and updating the internal model of the game environment based on the
    /// effects of those actions. This loop continues until manually stopped,
    /// or until an error occurs.
    /// </summary>
    public async Task Run()
    {
        if (IsRunning)
            throw new InvalidOperationException("Agent is already running.");

        IsRunning = true;

        _logger.LogInformation("Agent running.");
        
        string SerializeToJson(object obj) => JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        while (IsRunning)
        {
            _logger.LogInformation("Thinking...");

            var thoughtsContainer = await _agent.Think(new PedState
            {
                Health = 100
            });

            _logger.LogInformation(SerializeToJson(thoughtsContainer));

            ActionResult actionResult;

            if (thoughtsContainer.Command is null)
            {
                actionResult = new ActionResult(false, "No command given.");
            }
            else
            {
                var action = _actionLocator.Locate(thoughtsContainer.Command.Name);

                actionResult = action is null
                    ? new ActionResult(false, $"Command '{thoughtsContainer.Command.Name}' not found.")
                    : await _agent.Act(action, thoughtsContainer.Command.Args ?? new());
            }

            _logger.LogInformation(SerializeToJson(actionResult));

            _agent.Observe(actionResult);

            _logger.LogInformation("Chilling...");

            await Task.Delay(10000);
        }
    }

    /// <summary>
    /// Stops the autonomous pedestrian from executing the TAO loop.
    /// This method interrupts the `Run` method, preventing the pedestrian
    /// from generating any further plans, executing any further actions,
    /// or updating its internal model of the game environment. This method
    /// can be used to halt the execution of the TAO loop in response to a
    /// user input or other external event.
    /// </summary>
    public void Stop()
    {
        if (!IsRunning)
            throw new InvalidOperationException("Agent is not running.");

        IsRunning = false;
    }
}