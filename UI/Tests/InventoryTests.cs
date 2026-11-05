using System.Text.RegularExpressions;
using TestAutomation.UI.Base;
using TestAutomation.UI.Pages;
using TestAutomation.UI.TestData;

namespace TestAutomation.UI.Tests;

[Category("Inventory")]
public class InventoryTests : BaseTest
{
    private InventoryPage _inventoryPage = null!;

    [SetUp]
    public async Task LoginAsStandardUser()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.Standard);

        _inventoryPage = new InventoryPage(Page);
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("Adding a product shows a cart badge of 1 and switches its button to Remove")]
    public async Task AddProductToCart_UpdatesCartBadgeAndButton()
    {
        await Expect(_inventoryPage.CartBadge).Not.ToBeVisibleAsync();

        await _inventoryPage.AddToCartAsync(Products.Backpack);

        await Expect(_inventoryPage.CartBadge).ToHaveTextAsync("1");
        await Expect(_inventoryPage.ItemButton(Products.Backpack)).ToHaveTextAsync("Remove");
    }

    [Test]
    [Category("Positive")]
    [Description("Removing the only product hides the cart badge and resets its button to Add to cart")]
    public async Task RemoveProductFromCart_ClearsBadgeAndResetsButton()
    {
        await _inventoryPage.AddToCartAsync(Products.Backpack);
        await _inventoryPage.RemoveFromCartAsync(Products.Backpack);

        await Expect(_inventoryPage.CartBadge).Not.ToBeVisibleAsync();
        await Expect(_inventoryPage.ItemButton(Products.Backpack)).ToHaveTextAsync("Add to cart");
    }

    [Test]
    [Category("Positive")]
    [Description("Cart badge shows the number of products added")]
    public async Task AddMultipleProductsToCart_BadgeShowsCount()
    {
        Product[] products = [Products.Backpack, Products.BikeLight, Products.Onesie];

        foreach (var product in products)
            await _inventoryPage.AddToCartAsync(product);

        await Expect(_inventoryPage.CartBadge).ToHaveTextAsync(products.Length.ToString());
    }

    [Test]
    [Category("Positive")]
    [Description("Cart badge and button state survive a page reload")]
    public async Task CartContents_PersistAfterPageReload()
    {
        await _inventoryPage.AddToCartAsync(Products.Backpack);

        await Page.ReloadAsync();

        await Expect(_inventoryPage.CartBadge).ToHaveTextAsync("1");
        await Expect(_inventoryPage.ItemButton(Products.Backpack)).ToHaveTextAsync("Remove");
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("Clicking the cart icon opens the cart page with the added product")]
    public async Task ClickCartIcon_NavigatesToCartWithAddedProduct()
    {
        await _inventoryPage.AddToCartAsync(Products.Backpack);

        await _inventoryPage.OpenCartAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/cart\\.html$"));
        await Expect(new CartPage(Page).ItemNames).ToHaveTextAsync([Products.Backpack.Name]);
    }
}
