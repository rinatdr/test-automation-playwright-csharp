using TestAutomation.Api.Base;
using TestAutomation.Api.Models;
using TestAutomation.Api.TestData;

namespace TestAutomation.Api.Tests;

[Category("Auth")]
public class AuthTests : ApiTest
{
    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("POST /auth with valid credentials returns a token")]
    public async Task CreateToken_WithValidCredentials_ReturnsToken()
    {
        var response = await AuthClient.CreateTokenAsync(AdminCredentials.Valid);

        Assert.That(response.Status, Is.EqualTo(200));
        var auth = await response.ReadJsonAsync<AuthResponse>();
        Assert.That(auth.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    [Category("Negative")]
    [Description("POST /auth with a wrong password returns no token and a 'Bad credentials' reason")]
    public async Task CreateToken_WithWrongPassword_ReturnsNoToken()
    {
        var response = await AuthClient.CreateTokenAsync(AdminCredentials.WrongPassword);

        var auth = await response.ReadJsonAsync<AuthResponse>();
        Assert.That(auth.Token, Is.Null);
        Assert.That(auth.Reason, Is.EqualTo("Bad credentials"));
    }

    // KNOWN BUG: Restful-Booker answers 200 OK to a failed login.
    [Test]
    [Category("Negative"), Category("KnownBug")]
    [Description("POST /auth with a wrong password returns 401 Unauthorized")]
    public async Task CreateToken_WithWrongPassword_Returns401()
    {
        var response = await AuthClient.CreateTokenAsync(AdminCredentials.WrongPassword);

        Assert.That(response.Status, Is.EqualTo(401));
    }
}
