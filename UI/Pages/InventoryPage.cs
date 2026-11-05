using Microsoft.Playwright;
using TestAutomation.UI.TestData;

namespace TestAutomation.UI.Pages;

public class InventoryPage(IPage page)
{
    public ILocator CartBadge => page.Locator("[data-test='shopping-cart-badge']");
    private ILocator CartLink => page.Locator("[data-test='shopping-cart-link']");

    private ILocator Item(Product product) =>
        page.Locator("[data-test='inventory-item']").Filter(new() { HasText = product.Name });

    public ILocator ItemButton(Product product) => Item(product).Locator("button");

    // The same button toggles between "Add to cart" and "Remove".
    public async Task AddToCartAsync(Product product) => await ItemButton(product).ClickAsync();

    public async Task RemoveFromCartAsync(Product product) => await ItemButton(product).ClickAsync();

    public async Task OpenCartAsync() => await CartLink.ClickAsync();
}
