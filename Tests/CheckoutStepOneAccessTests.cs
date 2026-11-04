using System.Text.RegularExpressions;
using Swag_Labs.Base;
using Swag_Labs.Pages;
using Swag_Labs.TestData;

namespace Swag_Labs.Tests;

[Category("Checkout")]
public class CheckoutStepOneAccessTests : BaseTest
{
    private static readonly User[] UsersWithCheckoutBugs = [Users.Problem, Users.Error];

    [Test]
    [Category("Negative"), Category("Security")]
    [Description("Opening the checkout page directly without logging in redirects to the login page")]
    public async Task OpenDirectlyWithoutLogin_RedirectsToLogin()
    {
        var checkoutPage = new CheckoutStepOnePage(Page);
        var loginPage = new LoginPage(Page);

        await checkoutPage.GoToAsync();

        await Expect(Page).Not.ToHaveURLAsync(new Regex("/checkout-step-one\\.html$"));
        await Expect(loginPage.ErrorMessage).ToHaveTextAsync(
            "Epic sadface: You can only access '/checkout-step-one.html' when you are logged in.");
    }

    // KNOWN BUG: problem_user's last name is typed into the first name field,
    // and error_user's last name field does not accept input.
    [TestCaseSource(nameof(UsersWithCheckoutBugs))]
    [Category("Positive"), Category("KnownBug")]
    [Description("Typed values are kept in the form fields for every user")]
    public async Task FillInformation_FieldsKeepEnteredValues(User user)
    {
        var checkoutPage = await OpenCheckoutAsAsync(user);

        await checkoutPage.FillInformationAsync("John", "Doe", "12345");

        await Expect(checkoutPage.FirstNameInput).ToHaveValueAsync("John");
        await Expect(checkoutPage.LastNameInput).ToHaveValueAsync("Doe");
        await Expect(checkoutPage.PostalCodeInput).ToHaveValueAsync("12345");
    }

    // KNOWN BUG: error_user continues to checkout step two without a last name.
    [Test]
    [Category("Negative"), Category("KnownBug")]
    [Description("Continue without a last name as error_user stays on the checkout information page")]
    public async Task ContinueWithoutLastName_AsErrorUser_DoesNotNavigateToOverview()
    {
        var checkoutPage = await OpenCheckoutAsAsync(Users.Error);

        await checkoutPage.FillInformationAsync("John", "", "12345");
        await checkoutPage.ContinueAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/checkout-step-one\\.html$"));
        await Expect(checkoutPage.ErrorMessage).ToHaveTextAsync("Error: Last Name is required");
    }

    private async Task<CheckoutStepOnePage> OpenCheckoutAsAsync(User user)
    {
        var loginPage = new LoginPage(Page);
        await loginPage.GoToAsync();
        await loginPage.LoginAsync(user);

        var inventoryPage = new InventoryPage(Page);
        await inventoryPage.AddToCartAsync(Products.Backpack);
        await inventoryPage.OpenCartAsync();
        await new CartPage(Page).CheckoutAsync();

        return new CheckoutStepOnePage(Page);
    }
}
