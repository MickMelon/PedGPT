using System.Text.Json.Serialization;

namespace PedGPT.Infrastructure.OpenAi;

public record Choice(
    [property: JsonPropertyName("message")] Message Message,
    [property: JsonPropertyName("finish_reason")] string FinishReason,
    [property: JsonPropertyName("index")] int Index);