using Microsoft.Playwright;
using Swag_Labs.TestData;

namespace Swag_Labs.Pages;

public class CheckoutStepTwoPage(IPage page)
{
    public ILocator CartBadge => page.Locator("[data-test='shopping-cart-badge']");
    public ILocator ItemNames => page.Locator("[data-test='inventory-item-name']");
    public ILocator PaymentInfo => page.Locator("[data-test='payment-info-value']");
    public ILocator ShippingInfo => page.Locator("[data-test='shipping-info-value']");
    public ILocator SubtotalLabel => page.Locator("[data-test='subtotal-label']");
    public ILocator TaxLabel => page.Locator("[data-test='tax-label']");
    public ILocator TotalLabel => page.Locator("[data-test='total-label']");
    private ILocator FinishButton => page.Locator("[data-test='finish']");
    private ILocator CancelButton => page.Locator("[data-test='cancel']");

    private ILocator Item(Product product) =>
        page.Locator("[data-test='inventory-item']").Filter(new() { HasText = product.Name });

    public ILocator ItemPrice(Product product) => Item(product).Locator("[data-test='inventory-item-price']");

    public ILocator ItemQuantity(Product product) => Item(product).Locator("[data-test='item-quantity']");

    public async Task GoToAsync() => await page.GotoAsync("/checkout-step-two.html");

    public async Task FinishAsync() => await FinishButton.ClickAsync();

    public async Task CancelAsync() => await CancelButton.ClickAsync();
}
