using System.Text.Json.Serialization;

namespace PedGPT.Core.OpenAi;

public record Message(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")]
    string Content)
{
    public override string ToString()
    {
        return $"{Role}: {Content}";
    }
}