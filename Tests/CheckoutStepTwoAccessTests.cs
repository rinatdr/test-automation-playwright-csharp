using System.Text.RegularExpressions;
using Swag_Labs.Base;
using Swag_Labs.Pages;
using Swag_Labs.TestData;

namespace Swag_Labs.Tests;

[Category("Checkout")]
public class CheckoutStepTwoAccessTests : BaseTest
{
    [Test]
    [Category("Negative"), Category("Security")]
    [Description("Opening the checkout overview directly without logging in redirects to the login page")]
    public async Task OpenDirectlyWithoutLogin_RedirectsToLogin()
    {
        var overviewPage = new CheckoutStepTwoPage(Page);
        var loginPage = new LoginPage(Page);

        await overviewPage.GoToAsync();

        await Expect(Page).Not.ToHaveURLAsync(new Regex("/checkout-step-two\\.html$"));
        await Expect(loginPage.ErrorMessage).ToHaveTextAsync(
            "Epic sadface: You can only access '/checkout-step-two.html' when you are logged in.");
    }

    // KNOWN BUG: The overview opens directly by URL, skipping the customer information form.
    [Test]
    [Category("Negative"), Category("KnownBug")]
    [Description("Opening the checkout overview without filling in the customer information sends the user back to it")]
    public async Task OpenDirectlyWithoutCustomerInformation_RedirectsToStepOne()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.Standard);
        await new InventoryPage(Page).AddToCartAsync(Products.Backpack);

        await new CheckoutStepTwoPage(Page).GoToAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/checkout-step-one\\.html$"));
    }

    // KNOWN BUG: error_user clicks Finish and nothing happens.
    [Test]
    [Category("Positive"), Category("KnownBug")]
    [Description("Finish as error_user completes the order")]
    public async Task Finish_AsErrorUser_CompletesOrder()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.Error);

        var inventoryPage = new InventoryPage(Page);
        await inventoryPage.AddToCartAsync(Products.Backpack);
        await inventoryPage.OpenCartAsync();
        await new CartPage(Page).CheckoutAsync();

        var checkoutPage = new CheckoutStepOnePage(Page);
        await checkoutPage.FillInformationAsync("John", "Doe", "12345");
        await checkoutPage.ContinueAsync();

        await new CheckoutStepTwoPage(Page).FinishAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/checkout-complete\\.html$"));
    }
}
