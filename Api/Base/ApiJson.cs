using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Playwright;

namespace TestAutomation.Api.Base;

public static class ApiJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string Serialize(object body) => JsonSerializer.Serialize(body, body.GetType(), Options);

    public static async Task<T> ReadJsonAsync<T>(this IAPIResponse response)
    {
        var text = await response.TextAsync();
        return JsonSerializer.Deserialize<T>(text, Options)
               ?? throw new InvalidOperationException($"Could not read {typeof(T).Name} from: {text}");
    }
}
