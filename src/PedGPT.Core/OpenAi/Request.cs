using System.Text.Json.Serialization;

namespace PedGPT.Core.OpenAi;

public record Request(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("messages")] List<Message> Messages);