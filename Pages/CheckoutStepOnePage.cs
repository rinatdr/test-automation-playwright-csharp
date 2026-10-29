using Microsoft.Playwright;

namespace Swag_Labs.Pages;

public class CheckoutStepOnePage(IPage page)
{
    public ILocator FirstNameInput => page.Locator("[data-test='firstName']");
    public ILocator LastNameInput => page.Locator("[data-test='lastName']");
    public ILocator PostalCodeInput => page.Locator("[data-test='postalCode']");
    public ILocator ErrorMessage => page.Locator("[data-test='error']");
    private ILocator ContinueButton => page.Locator("[data-test='continue']");
    private ILocator CancelButton => page.Locator("[data-test='cancel']");

    public async Task GoToAsync() => await page.GotoAsync("/checkout-step-one.html");

    public async Task FillInformationAsync(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task ContinueAsync() => await ContinueButton.ClickAsync();

    public async Task CancelAsync() => await CancelButton.ClickAsync();

}
