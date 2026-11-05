using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using TestAutomation.Config;

namespace TestAutomation.UI.Base;

[Category("UI")]
public class BaseTest : PageTest
{
    protected static TestSettings Settings => TestSettings.Current;

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = Settings.UiBaseUrl
    };

    [SetUp]
    public void BaseSetUp()
    {
        Context.SetDefaultTimeout(Settings.DefaultTimeoutMs);
    }
}
