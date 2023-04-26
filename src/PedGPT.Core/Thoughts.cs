namespace PedGPT.Core;

public record Thoughts(
    string Text,
    string Reasoning,
    string Plan,
    string Criticism,
    string Speak);

public record Command(string Name, Dictionary<string, string>? Args);