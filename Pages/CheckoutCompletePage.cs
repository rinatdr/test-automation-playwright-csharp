using Microsoft.Playwright;

namespace Swag_Labs.Pages;

public class CheckoutCompletePage(IPage page)
{
    public ILocator CompleteHeader => page.Locator("[data-test='complete-header']");
}
