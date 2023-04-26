using System.Text.Json.Serialization;

namespace PedGPT.Core.OpenAi;

public record Response(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("object")] string Object,
    [property: JsonPropertyName("created")] int Created,
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("usage")] Usage Usage,
    [property: JsonPropertyName("choices")] List<Choice> Choices);