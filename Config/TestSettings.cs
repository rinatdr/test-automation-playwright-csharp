using System.Text.Json;

namespace TestAutomation.Config;

public record EnvironmentUrls(string Ui, string Api);

public class TestSettings
{
    public string DefaultEnvironment { get; init; } = "prod";
    public int DefaultTimeoutMs { get; init; } = 10000;
    public Dictionary<string, EnvironmentUrls> Environments { get; init; } = [];

    public static TestSettings Current { get; } = Load();

    public string UiBaseUrl => CurrentEnvironment.Ui;

    public string ApiBaseUrl => CurrentEnvironment.Api;

    // Selected by the "env" runsettings parameter or the TEST_ENV environment variable; defaults to DefaultEnvironment.
    private EnvironmentUrls CurrentEnvironment
    {
        get
        {
            var env = TestContext.Parameters.Get("env")
                      ?? Environment.GetEnvironmentVariable("TEST_ENV")
                      ?? DefaultEnvironment;

            return Environments.TryGetValue(env, out var urls)
                ? urls
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
