using System.Text.RegularExpressions;
using Swag_Labs.Base;
using Swag_Labs.Pages;
using Swag_Labs.TestData;

namespace Swag_Labs.Tests;

[Category("Login")]
public class LoginTests : BaseTest
{
    [TestCaseSource(typeof(Users), nameof(Users.CanLogin))]
    [Category("Positive"), Category("Smoke")]
    [Description("Every user allowed to log in reaches the inventory page")]
    public async Task Login_WithValidUser_NavigatesToInventory(User user)
    {
        var loginPage = new LoginPage(Page);

        await loginPage.GoToAsync();
        await loginPage.LoginAsync(user);

        await Expect(Page).ToHaveURLAsync(new Regex("/inventory.html"));
    }

    [Test]
    [Category("Negative")]
    [Description("Locked-out user sees an error and stays on the login page")]
    public async Task Login_WithLockedOutUser_ShowsErrorAndStaysOnLoginPage()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.GoToAsync();
        await loginPage.LoginAsync(Users.LockedOut);

        await Expect(loginPage.ErrorMessage).ToHaveTextAsync("Epic sadface: Sorry, this user has been locked out.");
        await Expect(Page).Not.ToHaveURLAsync(new Regex("/inventory.html"));
    }
}
