using System.Text.Json;

namespace PedGPT.Core.Json;

public class SystemTextJsonSerializer : IJsonSerializer
{
    private static JsonSerializerOptions Options => new()
    {
        Converters = { new ForceNumberToStringConverter() }
    };

    public string Serialize(object obj, bool format)
    {
        if (format) Options.WriteIndented = true;
        return JsonSerializer.Serialize(obj, Options);
    }

    public T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }
}