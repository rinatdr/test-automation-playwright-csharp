using System.Text.Json;

namespace Swag_Labs.Config;

public class TestSettings
{
    public string DefaultEnvironment { get; init; } = "prod";
    public int DefaultTimeoutMs { get; init; } = 10000;
    public Dictionary<string, string> Environments { get; init; } = [];

    public static TestSettings Current { get; } = Load();

    // Selected by the "env" runsettings parameter or the TEST_ENV environment variable; defaults to DefaultEnvironment.
    public string BaseUrl
    {
        get
        {
            var env = TestContext.Parameters.Get("env")
                      ?? Environment.GetEnvironmentVariable("TEST_ENV")
                      ?? DefaultEnvironment;

            return Environments.TryGetValue(env, out var url)
                ? url
                : throw new ArgumentException(
                    $"Unknown environment '{env}'. Valid values: {string.Join(", ", Environments.Keys)}");
        }
    }

    private static TestSettings Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Config", "appsettings.json");
        var settings = JsonSerializer.Deserialize<TestSettings>(File.ReadAllText(path))
                       ?? throw new InvalidOperationException($"Could not read {path}");

        return new TestSettings
        {
            DefaultEnvironment = settings.DefaultEnvironment,
            DefaultTimeoutMs = settings.DefaultTimeoutMs,
            Environments = new(settings.Environments, StringComparer.OrdinalIgnoreCase)
        };
    }
}
