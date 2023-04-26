using System.Text.Json.Serialization;

namespace PedGPT.Infrastructure.OpenAi;

public record Message(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content);