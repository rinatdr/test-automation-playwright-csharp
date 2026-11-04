using System.Globalization;
using System.Text.RegularExpressions;
using Swag_Labs.Base;
using Swag_Labs.Pages;
using Swag_Labs.TestData;

namespace Swag_Labs.Tests;

[Category("Checkout")]
public class CheckoutStepTwoTests : BaseTest
{
    private const decimal TaxRate = 0.08m;
    private static readonly Product[] OrderedProducts = [Products.Backpack, Products.BikeLight];

    private CheckoutStepTwoPage _overviewPage = null!;

    [SetUp]
    public async Task OpenOverviewWithProductsInCart()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.Standard);

        var inventoryPage = new InventoryPage(Page);
        foreach (var product in OrderedProducts)
            await inventoryPage.AddToCartAsync(product);
        await inventoryPage.OpenCartAsync();
        await new CartPage(Page).CheckoutAsync();

        var checkoutPage = new CheckoutStepOnePage(Page);
        await checkoutPage.FillInformationAsync("John", "Doe", "12345");
        await checkoutPage.ContinueAsync();

        _overviewPage = new CheckoutStepTwoPage(Page);
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("Overview lists each ordered product with its price and a quantity of 1")]
    public async Task Overview_ShowsOrderedProductsWithPriceAndQuantity()
    {
        await Expect(_overviewPage.ItemNames).ToHaveTextAsync(OrderedProducts.Select(p => p.Name));

        foreach (var product in OrderedProducts)
        {
            await Expect(_overviewPage.ItemPrice(product)).ToHaveTextAsync(Money(product.Price));
            await Expect(_overviewPage.ItemQuantity(product)).ToHaveTextAsync("1");
        }
    }

    [Test]
    [Category("Positive")]
    [Description("Overview shows the payment and shipping information")]
    public async Task Overview_ShowsPaymentAndShippingInformation()
    {
        await Expect(_overviewPage.PaymentInfo).ToHaveTextAsync("SauceCard #31337");
        await Expect(_overviewPage.ShippingInfo).ToHaveTextAsync("Free Pony Express Delivery!");
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("Item total is the sum of the prices, tax is 8% of it and total is their sum")]
    public async Task Overview_ShowsCorrectItemTotalTaxAndTotal()
    {
        var itemTotal = OrderedProducts.Sum(p => p.Price);
        var tax = Math.Round(itemTotal * TaxRate, 2);

        await Expect(_overviewPage.SubtotalLabel).ToHaveTextAsync("Item total: " + Money(itemTotal));
        await Expect(_overviewPage.TaxLabel).ToHaveTextAsync("Tax: " + Money(tax));
        await Expect(_overviewPage.TotalLabel).ToHaveTextAsync("Total: " + Money(itemTotal + tax));
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("Finish completes the order and empties the cart")]
    public async Task Finish_CompletesOrderAndEmptiesCart()
    {
        var completePage = new CheckoutCompletePage(Page);

        await _overviewPage.FinishAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/checkout-complete\\.html$"));
        await Expect(completePage.CompleteHeader).ToHaveTextAsync("Thank you for your order!");
        await Expect(_overviewPage.CartBadge).Not.ToBeVisibleAsync();
    }

    [Test]
    [Category("Positive")]
    [Description("Cancel returns to the inventory page and keeps the cart contents")]
    public async Task Cancel_ReturnsToInventoryAndKeepsCart()
    {
        await _overviewPage.CancelAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/inventory\\.html$"));
        await Expect(_overviewPage.CartBadge).ToHaveTextAsync(OrderedProducts.Length.ToString());
    }

    private static string Money(decimal amount) => "$" + amount.ToString("F2", CultureInfo.InvariantCulture);
}
