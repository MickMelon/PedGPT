using System.Text.Json;
using Microsoft.Extensions.Logging;
using PedGPT.Core.Commands;
using PedGPT.Core.Memories;
using PedGPT.Core.OpenAi;
using PedGPT.Core.Prompts;

namespace PedGPT.Core.Agents;

public class Agent
{
    public string Name { get; init; }
    public List<Goal> Goals { get; init; }
    public List<AgentState> States { get; init; }
    public List<CommandDescriptor> Commands { get; init; }
    public IMemory Memory;

    private readonly IOpenAiService _openAiService;
    private readonly ILogger<Agent> _logger;
    private readonly IPromptGenerator _promptGenerator;

    public Agent(
        string name,
        List<Goal> goals,
        List<AgentState> states,
        List<CommandDescriptor> commands,
        IMemory memory,
        IOpenAiService openAiService,
        ILogger<Agent> logger,
        IPromptGenerator promptGenerator)
    {
        Name = name;
        Goals = goals;
        States = states;
        Commands = commands;
        Memory = memory;
        _openAiService = openAiService;
        _logger = logger;
        _promptGenerator = promptGenerator;
    }

    public async Task<ThinkResult> Think()
    {
        var prompt = _promptGenerator.Generate(this);

        var messages = new List<Message>
        {
            new("system", prompt),
        };

        if (Memory.CommandsExecuted.Any()) 
            messages.Add(new("system", Memory.CommandsExecuted.Last().Value.Message));

        _logger.LogInformation(string.Join("\n\n", messages.Select(_ => _.ToString())));

        _logger.LogInformation("Thinking...");

        var response = await _openAiService.Completion(messages);

        var choice = response.Choices.First();
        
        var thinkResult = JsonSerializer.Deserialize<ThinkResult>(choice.Message.Content);

        _logger.LogInformation("Think result: {thinkResult}", SerializeToJson(thinkResult!));

        Memory.ThinkResults.Add(thinkResult!);

        return thinkResult!;
    }

    public async Task Act(string commandName, Dictionary<string, string> args)
    {
        var commandDescriptor = Commands.FirstOrDefault(_ => _.Name == commandName);

        if (commandDescriptor is null)
        {
            var commandNotFoundResult = new CommandResult(false, $"Unknown command '{commandName}'. Please refer to the 'Commands' list for available commands and only respond in the specified JSON format.");

            _logger.LogInformation("Command result: {commandResult}", SerializeToJson(commandNotFoundResult));

            Memory.CommandsExecuted.Add(new(commandName, commandNotFoundResult));

            return;
        }

        var command = commandDescriptor.ToCommand(args);

        var commandResult = await command.Execute();

        _logger.LogInformation("Command result: {commandResult}", SerializeToJson(commandResult));

        Memory.CommandsExecuted.Add(new(commandName, commandResult));
    }

    private static string SerializeToJson(object obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
    }
}