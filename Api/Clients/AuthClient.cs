using Microsoft.Playwright;
using TestAutomation.Api.Base;
using TestAutomation.Api.Models;

namespace TestAutomation.Api.Clients;

public class AuthClient(IAPIRequestContext request)
{
    public async Task<IAPIResponse> CreateTokenAsync(Credentials credentials) =>
        await request.PostAsync("/auth", new() { DataString = ApiJson.Serialize(credentials) });

    public async Task<string> GetTokenAsync(Credentials credentials)
    {
        var response = await CreateTokenAsync(credentials);
        var auth = await response.ReadJsonAsync<AuthResponse>();

        return auth.Token ?? throw new InvalidOperationException($"No token returned: {auth.Reason}");
    }
}
