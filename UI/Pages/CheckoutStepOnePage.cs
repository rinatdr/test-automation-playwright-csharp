using Microsoft.Playwright;

namespace TestAutomation.UI.Pages;

public class CheckoutStepOnePage(IPage page)
{
    public ILocator FirstNameInput => page.Locator("[data-test='firstName']");
    public ILocator LastNameInput => page.Locator("[data-test='lastName']");
    public ILocator PostalCodeInput => page.Locator("[data-test='postalCode']");
    public ILocator ErrorMessage => page.Locator("[data-test='error']");
    private ILocator ErrorCloseButton => page.Locator("[data-test='error-button']");
    private ILocator ContinueButton => page.Locator("[data-test='continue']");
    private ILocator CancelButton => page.Locator("[data-test='cancel']");
    private ILocator CartLink => page.Locator("[data-test='shopping-cart-link']");

    public async Task GoToAsync() => await page.GotoAsync("/checkout-step-one.html");

    public async Task FillInformationAsync(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task ContinueAsync() => await ContinueButton.ClickAsync();

    public async Task CancelAsync() => await CancelButton.ClickAsync();

    public async Task CloseErrorAsync() => await ErrorCloseButton.ClickAsync();

    public async Task OpenCartAsync() => await CartLink.ClickAsync();
}
