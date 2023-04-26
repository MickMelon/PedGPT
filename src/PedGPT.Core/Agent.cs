using System.Text.Json;
using Microsoft.Extensions.Logging;
using PedGPT.Core.Actions;
using PedGPT.Core.OpenAi;

namespace PedGPT.Core;

/// <summary>
/// Represents an autonomous pedestrian in a game environment that
/// uses the thought-action-observation (TAO) loop to operate. The
/// TAO loop consists of three stages: think, act, and observe. In
/// the think stage, the pedestrian generates plans and strategies
/// based on its perception of the game environment. In the act stage,
/// the pedestrian executes an action based on the plan generated in
/// the think stage. In the observe stage, the pedestrian updates its
/// internal model of the game environment based on the effects of its
/// actions.
/// </summary>
public class Agent
{
    private readonly Memory _memory;
    private readonly IOpenAiService _openAiService;
    private readonly ILogger<Agent> _logger;

    private ActionResult? _lastActionResult;

    public Agent(
        Memory memory, 
        IOpenAiService openAiService, 
        ILogger<Agent> logger)
    {
        _memory = memory;
        _openAiService = openAiService;
        _logger = logger;
    }

    /// <summary>
    /// Generates plans and strategies based on the agent's perception of the
    /// game environment.
    /// </summary>
    public async Task<ThinkResult> Think(PedState pedState)
    {
        var promptBuilder = new PromptBuilder()
            .WithState("Health", $"{pedState.Health}/100")
            //.WithState("Current vehicle", "Elegy")
            .WithCommand("dance", "Allows you to dance for joy!")
            //.WithCommand("look_at", "Looks at a position.")
            //.WithCommand("enter_vehicle", "Enters a vehicle with the given ID.")
            //.WithCommand("exit_vehicle", "Exits your current vehicle.")
            //.WithCommand("drive_to", "Drives to a position.")
            .WithCommand("walk", "Walks to a position (x, y, z).")
            //.WithCommand("run_to", "Runs to a position.")
            .WithConstraint("Must not use external APIs.")
            .WithConstraint("Only use resources provided.")
            .WithConstraint("Only one command can be used per resource.")
            .WithConstraint("Exclusively use the commands listed in double quotes e.g. \"command name\"");
            //.WithConstraint("If you are unsure how you previously did something or want to recall past events, thinking about similar events will help you remember..")
            //.WithPerformanceEvaluation("Continuously review and analyze your actions to ensure you are performing to the best of your abilities.")
            //.WithPerformanceEvaluation("Constructively self-criticize your big-picture behavior constantly.")
            //.WithPerformanceEvaluation("Reflect on past decisions and strategies to refine your approach.")
            //.WithPerformanceEvaluation("Be aware of your surroundings and the consequences of your actions.")
            //.WithPerformanceEvaluation("Every command has a cost, so be smart and efficient. Aim to complete tasks in the least number of steps.");

        foreach (var thinkResultMemory in _memory.ThinkResults)
        {
            promptBuilder.WithMemory(JsonSerializer.Serialize(thinkResultMemory));
        }

        var prompt = promptBuilder.Build();
        
        var messages = new List<Message>
        {
            new("system", prompt),
            new("user", _lastActionResult == null 
                ? "There is a car boot sale at Grove Street."
                : _lastActionResult.Message)
        };

        messages.ForEach(_ => _logger.LogInformation($"{_.Role}: {_.Content}"));

        var response = await _openAiService.Completion(messages);

        var choice = response.Choices.First();

        var thinkResult = JsonSerializer.Deserialize<ThinkResult>(choice.Message.Content);

        _memory.ThinkResults.Add(thinkResult!);

        return thinkResult!;
    }

    /// <summary>
    /// Executes an action based on the plan generated in the Think method.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <returns>The result from the action.</returns>
    public async Task<ActionResult> Act(IAction action, Dictionary<string, string> args)
    {
        return await action.Execute(args);
    }

    /// <summary>
    /// Updates the agent's internal model of the game environment based on the
    /// effects of its actions.
    /// </summary>
    public void Observe(ActionResult actionResult)
    {
        _lastActionResult = actionResult;
    }
}