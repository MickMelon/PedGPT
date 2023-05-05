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

    public PromptBuilder WithGoal(string description, double importance)
    {
        _goals.Add((description, importance));
        return this;
    }

    public string Build()
    {
        var prompt = new StringBuilder();

        prompt.AppendLine(_setup);

        if (_constraints.Any())
        {
            var constraints = new StringBuilder();
            constraints.AppendLine("Constraints:");
            foreach (var constraint in _constraints)
            {
                constraints.AppendLine($"- {constraint}");
            }
            prompt.AppendLine(constraints.ToString());
        }

        if (_performanceEvaluations.Any())
        {
            var performanceEvaluations = new StringBuilder();
            performanceEvaluations.AppendLine("Performance Evaluations:");
            foreach (var evaluation in _performanceEvaluations)
            {
                performanceEvaluations.AppendLine($"- {evaluation}");
            }
            prompt.AppendLine(performanceEvaluations.ToString());
        }

        if (_commands.Any())
        {
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
            prompt.AppendLine(commands.ToString());
        }

        if (_states.Any())
        {
            var states = new StringBuilder();
            states.AppendLine("Your Current State:");
            foreach (var (name, state) in _states)
            {
                states.AppendLine($"- {name}: {state}");
            }
            prompt.AppendLine(states.ToString());
        }

        if (_goals.Any())
        {
            var goals = new StringBuilder();
            goals.AppendLine("Your Goals (highest importance first):");
            foreach (var (description, importance) in _goals.OrderByDescending(_ => _.Item2))
            {
                goals.AppendLine($"- {description} (importance: {importance})");
            }
            prompt.AppendLine(goals.ToString());
        }
        
        prompt.AppendLine(_responseFormat);

        return prompt.ToString();
    }
}