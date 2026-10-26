using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Swag_Labs.Config;

namespace Swag_Labs.Base;

public class BaseTest : PageTest
{
    protected static TestSettings Settings => TestSettings.Current;

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = Settings.BaseUrl
    };

    [SetUp]
    public void BaseSetUp()
    {
        Context.SetDefaultTimeout(Settings.DefaultTimeoutMs);
    }
}
