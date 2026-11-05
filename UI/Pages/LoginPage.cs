using Microsoft.Playwright;
using TestAutomation.UI.TestData;

namespace TestAutomation.UI.Pages;

public class LoginPage(IPage page)
{
    private ILocator UsernameInput => page.Locator("[data-test='username']");
    private ILocator PasswordInput => page.Locator("[data-test='password']");
    private ILocator LoginButton => page.Locator("[data-test='login-button']");
    public ILocator ErrorMessage => page.Locator("[data-test='error']");

    public async Task GoToAsync() => await page.GotoAsync("/");

    public async Task LoginAsync(User user)
    {
        await UsernameInput.FillAsync(user.Username);
        await PasswordInput.FillAsync(user.Password);
        await LoginButton.ClickAsync();
    }
}
