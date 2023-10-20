using Microsoft.Extensions.Logging;
using PedGPT.Core.Commands;
using PedGPT.Core.Json;
using PedGPT.Core.Memories;
using PedGPT.Core.OpenAi;
using PedGPT.Core.Prompts;

namespace PedGPT.Core.Agents;

public class AgentBuilder
{
    private string _name = "Agent";
    private readonly List<Goal> _goals = new();
    private readonly List<AgentState> _states = new();
    private readonly List<CommandDescriptor> _commands = new();
    private IMemoryStorage? _memory;
    private IOpenAiService? _openAiService;
    private ILogger<Agent>? _logger;
    private IPromptGenerator? _promptGenerator;
    private IJsonSerializer? _jsonSerializer;

    public AgentBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public AgentBuilder WithGoal(Goal goal)
    {
        _goals.Add(goal);
        return this;
    }

    public AgentBuilder WithState(AgentState state)
    {
        _states.Add(state);
        return this;
    }

    public AgentBuilder WithCommand<T>() where T : ICommand
    {
        _commands.Add(CommandDescriptor.Create<T>());
        return this;
    }

    public AgentBuilder WithMemory(IMemoryStorage memory)
    {
        _memory = memory;
        return this;
    }

    public AgentBuilder WithOpenAiService(IOpenAiService openAiService)
    {
        _openAiService = openAiService;
        return this;
    }

    public AgentBuilder WithLogger(ILogger<Agent> logger)
    {
        _logger = logger;
        return this;
    }

    public AgentBuilder WithPromptGenerator(IPromptGenerator promptGenerator)
    {
        _promptGenerator = promptGenerator;
        return this;
    }

    public AgentBuilder WithJsonSerializer(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
        return this;
    }

    public Agent Build()
    {
        if (_memory is null) throw new InvalidOperationException("Agent requires memory storage.");
        if (_openAiService is null) throw new InvalidOperationException("Agent requires OpenAI service.");
        if (_logger is null) throw new InvalidOperationException("Agent requires logger.");
        if (_promptGenerator is null) throw new InvalidOperationException("Agent requires prompt generator.");
        if (_jsonSerializer is null) throw new InvalidOperationException("Agent requires JSON serializer.");

        return new(
            _name, 
            _goals, 
            _states,
            _commands,
            _memory,
            _openAiService,
            _logger,
            _promptGenerator,
            _jsonSerializer);
    }
}