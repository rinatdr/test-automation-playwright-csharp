using TestAutomation.Api.Base;
using TestAutomation.Api.Models;
using TestAutomation.Api.TestData;

namespace TestAutomation.Api.Tests;

[Category("Booking")]
public class UpdateBookingTests : ApiTest
{
    private const int NonExistentId = 999999999;

    private Booking _original = null!;
    private int _bookingId;

    [SetUp]
    public async Task CreateBookingToUpdate()
    {
        _original = BookingData.NewBooking();
        _bookingId = (await CreateBookingAsync(_original)).BookingId;
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("PUT /booking/{id} with a token replaces every field of the booking")]
    public async Task UpdateBooking_WithToken_ReplacesAllFields()
    {
        var updated = BookingData.UpdatedBooking();

        var response = await BookingClient.UpdateBookingAsync(_bookingId, updated, await GetAdminTokenAsync());

        Assert.That(response.Status, Is.EqualTo(200));
        Assert.That(await response.ReadJsonAsync<Booking>(), Is.EqualTo(updated));
        Assert.That(await GetStoredBookingAsync(), Is.EqualTo(updated));
    }

    [Test]
    [Category("Negative"), Category("Security")]
    [Description("PUT /booking/{id} without a token is rejected and the booking is not changed")]
    public async Task UpdateBooking_WithoutToken_IsRejectedAndBookingUnchanged()
    {
        var response = await BookingClient.UpdateBookingAsync(_bookingId, BookingData.UpdatedBooking(), token: null);

        Assert.That(response.Ok, Is.False, $"Expected a rejection, got {response.Status}");
        Assert.That(await GetStoredBookingAsync(), Is.EqualTo(_original));
    }

    [Test]
    [Category("Negative"), Category("Security")]
    [Description("PUT /booking/{id} with an invalid token is rejected and the booking is not changed")]
    public async Task UpdateBooking_WithInvalidToken_IsRejectedAndBookingUnchanged()
    {
        var response = await BookingClient.UpdateBookingAsync(_bookingId, BookingData.UpdatedBooking(), "invalid-token");

        Assert.That(response.Ok, Is.False, $"Expected a rejection, got {response.Status}");
        Assert.That(await GetStoredBookingAsync(), Is.EqualTo(_original));
    }

    [Test]
    [Category("Negative")]
    [Description("PUT /booking/{id} with missing required fields returns 400 and the booking is not changed")]
    public async Task UpdateBooking_WithMissingRequiredFields_Returns400()
    {
        var response = await BookingClient.UpdateBookingAsync(
            _bookingId, new { firstname = "OnlyFirstName" }, await GetAdminTokenAsync());

        Assert.That(response.Status, Is.EqualTo(400));
        Assert.That(await GetStoredBookingAsync(), Is.EqualTo(_original));
    }

    // KNOWN BUG: Restful-Booker answers 403 Forbidden when no token is sent at all.
    [Test]
    [Category("Negative"), Category("KnownBug")]
    [Description("PUT /booking/{id} without a token returns 401 Unauthorized")]
    public async Task UpdateBooking_WithoutToken_Returns401()
    {
        var response = await BookingClient.UpdateBookingAsync(_bookingId, BookingData.UpdatedBooking(), token: null);

        Assert.That(response.Status, Is.EqualTo(401));
    }

    // KNOWN BUG: Restful-Booker answers 405 Method Not Allowed for a booking that does not exist.
    [Test]
    [Category("Negative"), Category("KnownBug")]
    [Description("PUT /booking/{id} for an ID that does not exist returns 404")]
    public async Task UpdateBooking_NonExistentId_Returns404()
    {
        var response = await BookingClient.UpdateBookingAsync(
            NonExistentId, BookingData.UpdatedBooking(), await GetAdminTokenAsync());

        Assert.That(response.Status, Is.EqualTo(404));
    }

    private async Task<Booking> GetStoredBookingAsync() =>
        await (await BookingClient.GetBookingAsync(_bookingId)).ReadJsonAsync<Booking>();
}
