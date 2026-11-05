using System.Text.RegularExpressions;
using TestAutomation.UI.Base;
using TestAutomation.UI.Pages;
using TestAutomation.UI.TestData;

namespace TestAutomation.UI.Tests;

[Category("Checkout")]
public class CheckoutStepOneTests : BaseTest
{
    private CartPage _cartPage = null!;
    private CheckoutStepOnePage _checkoutPage = null!;

    [SetUp]
    public async Task OpenCheckoutWithProductInCart()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.Standard);

        var inventoryPage = new InventoryPage(Page);
        _cartPage = new CartPage(Page);
        _checkoutPage = new CheckoutStepOnePage(Page);

        await inventoryPage.AddToCartAsync(Products.Backpack);
        await inventoryPage.OpenCartAsync();
        await _cartPage.CheckoutAsync();
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("Continue with all fields filled opens the checkout overview page")]
    public async Task ContinueWithValidInformation_NavigatesToOverviewPage()
    {
        await _checkoutPage.FillInformationAsync("John", "Doe", "12345");

        await _checkoutPage.ContinueAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/checkout-step-two\\.html$"));
    }

    [Test]
    [Category("Positive")]
    [Description("Typed values are kept in the form fields")]
    public async Task FillInformation_FieldsKeepEnteredValues()
    {
        await _checkoutPage.FillInformationAsync("John", "Doe", "12345");

        await Expect(_checkoutPage.FirstNameInput).ToHaveValueAsync("John");
        await Expect(_checkoutPage.LastNameInput).ToHaveValueAsync("Doe");
        await Expect(_checkoutPage.PostalCodeInput).ToHaveValueAsync("12345");
    }

    [TestCase("", "Doe", "12345", "Error: First Name is required")]
    [TestCase("John", "", "12345", "Error: Last Name is required")]
    [TestCase("John", "Doe", "", "Error: Postal Code is required")]
    [TestCase("", "", "", "Error: First Name is required")]
    [Category("Negative")]
    [Description("Continue with a missing required field shows an error and stays on the page")]
    public async Task ContinueWithMissingField_ShowsError(
        string firstName, string lastName, string postalCode, string expectedError)
    {
        await _checkoutPage.FillInformationAsync(firstName, lastName, postalCode);

        await _checkoutPage.ContinueAsync();

        await Expect(_checkoutPage.ErrorMessage).ToHaveTextAsync(expectedError);
        await Expect(Page).ToHaveURLAsync(new Regex("/checkout-step-one\\.html$"));
    }

    [Test]
    [Category("Negative")]
    [Description("Closing the error message hides it")]
    public async Task CloseError_HidesErrorMessage()
    {
        await _checkoutPage.ContinueAsync();
        await Expect(_checkoutPage.ErrorMessage).ToBeVisibleAsync();

        await _checkoutPage.CloseErrorAsync();

        await Expect(_checkoutPage.ErrorMessage).ToBeHiddenAsync();
    }

    [Test]
    [Category("Positive")]
    [Description("Cancel returns to the cart page and keeps the cart contents")]
    public async Task Cancel_ReturnsToCartAndKeepsItems()
    {
        await _checkoutPage.CancelAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/cart\\.html$"));
        await Expect(_cartPage.ItemNames).ToHaveTextAsync([Products.Backpack.Name]);
        await Expect(_cartPage.CartBadge).ToHaveTextAsync("1");
    }

    [Test]
    [Category("Positive")]
    [Description("Cart icon in the header opens the cart page and keeps the cart contents")]
    public async Task CartIcon_OpensCartAndKeepsItems()
    {
        await _checkoutPage.OpenCartAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/cart\\.html$"));
        await Expect(_cartPage.ItemNames).ToHaveTextAsync([Products.Backpack.Name]);
        await Expect(_cartPage.CartBadge).ToHaveTextAsync("1");
    }
}
