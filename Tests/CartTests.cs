using System.Globalization;
using System.Text.RegularExpressions;
using Swag_Labs.Base;
using Swag_Labs.Pages;
using Swag_Labs.TestData;

namespace Swag_Labs.Tests;

[Category("Cart")]
public class CartTests : BaseTest
{
    private InventoryPage _inventoryPage = null!;
    private CartPage _cartPage = null!;

    [SetUp]
    public async Task LoginAsStandardUser()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.Standard);

        _inventoryPage = new InventoryPage(Page);
        _cartPage = new CartPage(Page);
    }

    [Test]
    [Category("Positive")]
    [Description("Opening the cart with nothing added shows no items and no badge")]
    public async Task EmptyCart_ShowsNoItems()
    {
        await _cartPage.GoToAsync();

        await Expect(_cartPage.ItemNames).ToHaveCountAsync(0);
        await Expect(_cartPage.CartBadge).Not.ToBeVisibleAsync();
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("Cart lists each added product with its price and a quantity of 1")]
    public async Task Cart_ShowsAddedProductsWithPriceAndQuantity()
    {
        Product[] products = [Products.Backpack, Products.BikeLight];
        foreach (var product in products)
            await _inventoryPage.AddToCartAsync(product);

        await _inventoryPage.OpenCartAsync();

        await Expect(_cartPage.ItemNames).ToHaveTextAsync(products.Select(p => p.Name));
        foreach (var product in products)
        {
            await Expect(_cartPage.ItemPrice(product))
                .ToHaveTextAsync("$" + product.Price.ToString(CultureInfo.InvariantCulture));
            await Expect(_cartPage.ItemQuantity(product)).ToHaveTextAsync("1");
        }
    }

    [Test]
    [Category("Positive")]
    [Description("Removing a product from the cart page removes only that item and updates the badge")]
    public async Task RemoveProductInCart_RemovesItAndUpdatesBadge()
    {
        await _inventoryPage.AddToCartAsync(Products.Backpack);
        await _inventoryPage.AddToCartAsync(Products.Onesie);
        await _inventoryPage.OpenCartAsync();

        await _cartPage.RemoveAsync(Products.Backpack);

        await Expect(_cartPage.ItemNames).ToHaveTextAsync([Products.Onesie.Name]);
        await Expect(_cartPage.CartBadge).ToHaveTextAsync("1");
    }

    [Test]
    [Category("Positive")]
    [Description("Continue Shopping returns to the inventory page and keeps the cart contents")]
    public async Task ContinueShopping_ReturnsToInventoryAndKeepsCart()
    {
        await _inventoryPage.AddToCartAsync(Products.Backpack);
        await _inventoryPage.OpenCartAsync();

        await _cartPage.ContinueShoppingAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/inventory\\.html$"));
        await Expect(_inventoryPage.CartBadge).ToHaveTextAsync("1");
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("Checkout from the cart opens the checkout information page")]
    public async Task Checkout_NavigatesToCheckoutInformationPage()
    {
        await _inventoryPage.AddToCartAsync(Products.Backpack);
        await _inventoryPage.OpenCartAsync();

        await _cartPage.CheckoutAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/checkout-step-one\\.html$"));
    }

    [Test]
    [Category("Positive")]
    [Description("Cart contents persist after reloading the cart page")]
    public async Task ReloadCartPage_KeepsCartContents()
    {
        Product[] products = [Products.Backpack, Products.FleeceJacket];
        foreach (var product in products)
            await _inventoryPage.AddToCartAsync(product);
        await _inventoryPage.OpenCartAsync();

        await Page.ReloadAsync();

        await Expect(_cartPage.ItemNames).ToHaveTextAsync(products.Select(p => p.Name));
        await Expect(_cartPage.CartBadge).ToHaveTextAsync(products.Length.ToString());
    }

    // KNOWN BUG: Swag Labs lets the user continue to checkout step one from an empty cart.
    [Test]
    [Category("Negative"), Category("Edge"), Category("KnownBug")]
    [Description("Checkout from an empty cart is blocked and the user stays on the cart page")]
    public async Task CheckoutWithEmptyCart_StaysOnCartPage()
    {
        await _cartPage.GoToAsync();
        await Expect(_cartPage.ItemNames).ToHaveCountAsync(0);

        await _cartPage.CheckoutAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/cart\\.html$"));
    }

    [Test]
    [Category("Edge")]
    [Description("Adding every available product lists all of them in the cart and sets the badge to the full count")]
    public async Task AddAllProducts_CartListsEveryProduct()
    {
        foreach (var product in Products.All)
            await _inventoryPage.AddToCartAsync(product);

        await _inventoryPage.OpenCartAsync();

        await Expect(_cartPage.ItemNames).ToHaveTextAsync(Products.All.Select(p => p.Name));
        await Expect(_cartPage.CartBadge).ToHaveTextAsync(Products.All.Length.ToString());
    }

    [Test]
    [Category("Edge")]
    [Description("Removing every item one by one updates the list and badge each time until the cart is empty")]
    public async Task RemoveAllProductsSequentially_EmptiesCart()
    {
        foreach (var product in Products.All)
            await _inventoryPage.AddToCartAsync(product);
        await _inventoryPage.OpenCartAsync();

        for (var i = 0; i < Products.All.Length; i++)
        {
            await _cartPage.RemoveAsync(Products.All[i]);

            var remaining = Products.All[(i + 1)..];
            await Expect(_cartPage.ItemNames).ToHaveTextAsync(remaining.Select(p => p.Name));
            if (remaining.Length > 0)
                await Expect(_cartPage.CartBadge).ToHaveTextAsync(remaining.Length.ToString());
        }

        await Expect(_cartPage.ItemNames).ToHaveCountAsync(0);
        await Expect(_cartPage.CartBadge).Not.ToBeVisibleAsync();
    }
}
