namespace PedGPT.Core.Json;

public interface IJsonSerializer
{
    string Serialize(object obj);
    T? Deserialize<T>(string json);
}