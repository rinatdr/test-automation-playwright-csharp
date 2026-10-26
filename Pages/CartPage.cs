using Microsoft.Playwright;
using Swag_Labs.TestData;

namespace Swag_Labs.Pages;

public class CartPage(IPage page)
{
    public ILocator CartBadge => page.Locator("[data-test='shopping-cart-badge']");
    public ILocator ItemNames => page.Locator("[data-test='inventory-item-name']");
    private ILocator ContinueShoppingButton => page.Locator("[data-test='continue-shopping']");
    private ILocator CheckoutButton => page.Locator("[data-test='checkout']");

    private ILocator Item(Product product) =>
        page.Locator("[data-test='inventory-item']").Filter(new() { HasText = product.Name });

    public ILocator ItemPrice(Product product) => Item(product).Locator("[data-test='inventory-item-price']");

    public ILocator ItemQuantity(Product product) => Item(product).Locator("[data-test='item-quantity']");

    public async Task GoToAsync() => await page.GotoAsync("/cart.html");

    public async Task RemoveAsync(Product product) => await Item(product).Locator("button").ClickAsync();

    public async Task ContinueShoppingAsync() => await ContinueShoppingButton.ClickAsync();

    public async Task CheckoutAsync() => await CheckoutButton.ClickAsync();
}
