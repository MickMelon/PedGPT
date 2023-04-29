using System.Text;

namespace PedGPT.Core.Prompts;

public class PromptBuilder
{
    private string _setup = "";
    private string _responseFormat = "";
    private readonly List<(string, string, Dictionary<string, string>?)> _commands = new();
    private readonly List<(string, string)> _states = new();
    private readonly List<string> _constraints = new();
    private readonly List<string> _performanceEvaluations = new();
    private readonly List<string> _memories = new();
    private readonly List<(string, double)> _goals = new();

    public PromptBuilder WithSetup(string setup)
    {
        _setup = setup;
        return this;
    }

    public PromptBuilder WithResponseFormat(string responseFormat)
    {
        _responseFormat = responseFormat;
        return this;
    }

    public PromptBuilder WithCommand(string name, string description, Dictionary<string, string>? argsDescriptions)
    {
        _commands.Add((name, description, argsDescriptions));
        return this;
    }

    public PromptBuilder WithState(string name, string state)
    {
        _states.Add((name, state));
        return this;
    }

    public PromptBuilder WithConstraints(string[] constraints)
    {
        _constraints.AddRange(constraints);
        return this;
    }

    public PromptBuilder WithPerformanceEvaluations(string[] evaluations)
    {
        _performanceEvaluations.AddRange(evaluations);
        return this;
    }

    public PromptBuilder WithMemories(IEnumerable<string> memories)
    {
        _memories.AddRange(memories);
        return this;
    }

    public PromptBuilder WithGoal(string description, double importance)
    {
        _goals.Add((description, importance));
        return this;
    }

    public string Build()
    {
        var constraints = new StringBuilder();
        constraints.AppendLine("Constraints:");
        foreach (var constraint in _constraints)
        {
            constraints.AppendLine($"- {constraint}");
        }

        var performanceEvaluations = new StringBuilder();
        performanceEvaluations.AppendLine("Performance Evaluations:");
        foreach (var evaluation in _performanceEvaluations)
        {
            performanceEvaluations.AppendLine($"- {evaluation}");
        }

        var commands = new StringBuilder();
        commands.AppendLine("Commands:");
        for (var i = 0; i < _commands.Count; i++)
        {
            var (name, description, argsDescriptions) = _commands[i];

            var argsDescriptionsStr = argsDescriptions is not null && argsDescriptions.Any()
                ? string.Join(", ", argsDescriptions.Select(_ => $"\"{_.Key}\": \"{_.Value}\""))
                : "none";

            commands.AppendLine($"{i + 1}. {name}: \"{name}\", desc: \"{description}\", args: {argsDescriptionsStr}");
        }

        var states = new StringBuilder();
        states.AppendLine("Your Current State:");
        foreach (var (name, state) in _states)
        {
            states.AppendLine($"- {name}: {state}");
        }

        var memories = new StringBuilder();
        memories.AppendLine("Your Memories (last item is most recent):");
        foreach (var memory in _memories)
        {
            memories.AppendLine($"- {memory}");
        }

        var goals = new StringBuilder();
        goals.AppendLine("Your Goals (highest importance first):");
        foreach (var (description, importance) in _goals.OrderByDescending(_ => _.Item2))
        {
            goals.AppendLine($"- {description} (importance: {importance})");
        }

        var prompt = new StringBuilder();

        prompt.AppendLine(_setup);
        prompt.AppendLine(constraints.ToString());
        prompt.AppendLine(commands.ToString());
        prompt.AppendLine(states.ToString());
        prompt.AppendLine(performanceEvaluations.ToString());
        prompt.AppendLine(memories.ToString());
        prompt.AppendLine(goals.ToString());
        prompt.AppendLine(_responseFormat);
        prompt.AppendLine();

        return prompt.ToString();
    }
}