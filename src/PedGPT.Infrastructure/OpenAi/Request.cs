using System.Text.Json.Serialization;

namespace PedGPT.Infrastructure.OpenAi;

public record Request(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("messages")] List<Message> Messages);