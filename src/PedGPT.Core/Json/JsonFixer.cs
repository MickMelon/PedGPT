namespace PedGPT.Core.Json;

public static class JsonFixer
{
    public static string FixJson(string json)
    {
        json = json.Replace("\\'", "'");
        json = RemoveTextBeforeActualJsonStarts(json);
        return json;
    }

    /// <summary>
    /// Sometimes it responds with a string that contains text before the JSON starts.
    /// e.g. "Response:\r\n{\r\n    \"Thoughts\": {\r\n  ..."
    /// </summary>
    private static string RemoveTextBeforeActualJsonStarts(string json)
    {
        var indexOfFirstCurlyBracket = json.IndexOf('{');

        return indexOfFirstCurlyBracket == -1 ? json : json[indexOfFirstCurlyBracket..];
    }
}