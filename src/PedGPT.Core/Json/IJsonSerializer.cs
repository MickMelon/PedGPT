namespace PedGPT.Core.Json;

public interface IJsonSerializer
{
    string Serialize(object obj, bool format = false);
    T? Deserialize<T>(string json);
}