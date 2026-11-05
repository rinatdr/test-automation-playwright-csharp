using Microsoft.Playwright;

namespace TestAutomation.UI.Pages;

public class CheckoutCompletePage(IPage page)
{
    public ILocator CompleteHeader => page.Locator("[data-test='complete-header']");
}
