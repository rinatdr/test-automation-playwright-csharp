using Microsoft.Playwright;
using TestAutomation.Api.Base;

namespace TestAutomation.Api.Clients;

public class BookingClient(IAPIRequestContext request)
{
    public async Task<IAPIResponse> GetBookingIdsAsync(string? firstName = null, string? lastName = null)
    {
        var query = new List<string>();
        if (firstName is not null) query.Add("firstname=" + Uri.EscapeDataString(firstName));
        if (lastName is not null) query.Add("lastname=" + Uri.EscapeDataString(lastName));

        return await request.GetAsync(query.Count == 0 ? "/booking" : "/booking?" + string.Join("&", query));
    }

    public async Task<IAPIResponse> GetBookingAsync(int id) => await request.GetAsync($"/booking/{id}");

    // Takes any object so tests can also send invalid bodies.
    public async Task<IAPIResponse> CreateBookingAsync(object body) =>
        await request.PostAsync("/booking", new() { DataString = ApiJson.Serialize(body) });

    public async Task<IAPIResponse> UpdateBookingAsync(int id, object body, string? token) =>
        await request.PutAsync($"/booking/{id}", new()
        {
            DataString = ApiJson.Serialize(body),
            Headers = TokenHeader(token)
        });

    public async Task<IAPIResponse> DeleteBookingAsync(int id, string? token) =>
        await request.DeleteAsync($"/booking/{id}", new() { Headers = TokenHeader(token) });

    // Restful-Booker reads the token from a cookie, not from an Authorization header.
    private static Dictionary<string, string> TokenHeader(string? token) =>
        token is null ? [] : new() { ["Cookie"] = $"token={token}" };
}
