using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using TestAutomation.Api.Clients;
using TestAutomation.Api.Models;
using TestAutomation.Api.TestData;
using TestAutomation.Config;

namespace TestAutomation.Api.Base;

[Category("Api")]
public class ApiTest : PlaywrightTest
{
    protected static TestSettings Settings => TestSettings.Current;

    private IAPIRequestContext _request = null!;
    private readonly List<int> _createdBookingIds = [];

    protected AuthClient AuthClient { get; private set; } = null!;
    protected BookingClient BookingClient { get; private set; } = null!;

    [SetUp]
    public async Task CreateRequestContext()
    {
        _request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = Settings.ApiBaseUrl,
            Timeout = Settings.DefaultTimeoutMs,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json"
            }
        });

        AuthClient = new AuthClient(_request);
        BookingClient = new BookingClient(_request);
    }

    // The API is public and shared, so every booking a test creates is deleted afterwards.
    [TearDown]
    public async Task DeleteCreatedBookingsAndDispose()
    {
        if (_createdBookingIds.Count > 0)
        {
            var token = await GetAdminTokenAsync();
            foreach (var id in _createdBookingIds)
                await BookingClient.DeleteBookingAsync(id, token);
        }

        await _request.DisposeAsync();
    }

    protected async Task<string> GetAdminTokenAsync() => await AuthClient.GetTokenAsync(AdminCredentials.Valid);

    // Sends POST /booking and, if a booking was created, registers it for cleanup.
    protected async Task<IAPIResponse> PostBookingAsync(object body)
    {
        var response = await BookingClient.CreateBookingAsync(body);
        if (response.Ok)
            _createdBookingIds.Add((await response.ReadJsonAsync<CreatedBooking>()).BookingId);

        return response;
    }

    protected async Task<CreatedBooking> CreateBookingAsync(Booking booking)
    {
        var response = await PostBookingAsync(booking);
        Assert.That(response.Status, Is.EqualTo(200), "Creating the test booking failed");

        return await response.ReadJsonAsync<CreatedBooking>();
    }
}
