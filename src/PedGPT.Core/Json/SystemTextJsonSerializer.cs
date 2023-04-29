using System.Text.Json;

namespace PedGPT.Core.Json;

public class SystemTextJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions? _options = new()
    {
        Converters = { new ForceNumberToStringConverter() }
    };

    public string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    public T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _options);
    }
}