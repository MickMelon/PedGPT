using Microsoft.Extensions.Logging;
using PedGPT.Core.Commands;
using PedGPT.Core.Json;
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
    public IMemoryStorage MemoryStorage;

    private readonly IOpenAiService _openAiService;
    private readonly ILogger<Agent> _logger;
    private readonly IPromptGenerator _promptGenerator;
    private readonly IJsonSerializer _jsonSerializer;

    public Agent(
        string name,
        List<Goal> goals,
        List<AgentState> states,
        List<CommandDescriptor> commands,
        IMemoryStorage memoryStorage,
        IOpenAiService openAiService,
        ILogger<Agent> logger,
        IPromptGenerator promptGenerator,
        IJsonSerializer jsonSerializer)
    {
        Name = name;
        Goals = goals;
        States = states;
        Commands = commands;
        MemoryStorage = memoryStorage;
        _openAiService = openAiService;
        _logger = logger;
        _promptGenerator = promptGenerator;
        _jsonSerializer = jsonSerializer;
    }

    public async Task<ThinkResult> Think()
    {
        string prompt = _promptGenerator.Generate(this);

        List<Message> messages = new List<Message> { new("system", prompt) };

        MemoryStorage.Memories.ForEach(memory =>
        {
            messages.Add(new("assistant", _jsonSerializer.Serialize(memory.ThinkResult)));
            messages.Add(new("user", memory.ActResult.CommandResult.Message));
        });

        _logger.LogInformation(string.Join("\n\n", messages.Select(_ => _.ToString())));

        _logger.LogInformation("Thinking...");

        Response response = await _openAiService.Completion(messages);

        Choice choice = response.Choices.First();

        string fixedJson = JsonFixer.FixJson(choice.Message.Content);
        
        ThinkResult? thinkResult = _jsonSerializer.Deserialize<ThinkResult>(fixedJson);

        _logger.LogInformation("Think result: {thinkResult}", _jsonSerializer.Serialize(thinkResult!, format: true));

        return thinkResult!;
    }

    public async Task<ActResult> Act(string commandName, Dictionary<string, string> args)
    {
        CommandDescriptor? commandDescriptor = Commands.FirstOrDefault(_ => _.Name == commandName);

        if (commandDescriptor is null)
        {
            CommandResult commandNotFoundResult = new CommandResult(false, $"Unknown command '{commandName}'. Please refer to the 'Commands' list for available commands and only respond in the specified JSON format.");

            _logger.LogInformation("Command result: {commandResult}", _jsonSerializer.Serialize(commandNotFoundResult, format: true));

            return new(commandName, commandNotFoundResult);
        }

        ICommand command = commandDescriptor.ToCommand(args);

        CommandResult commandResult = await command.Execute();

        _logger.LogInformation("Command result: {commandResult}", _jsonSerializer.Serialize(commandResult, format: true));

        return new(commandName, commandResult);
    }

    public void Observe(ThinkResult thinkResult, ActResult actResult)
    {
        MemoryStorage.Add(thinkResult, actResult);
    }
}