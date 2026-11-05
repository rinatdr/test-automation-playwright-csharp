using TestAutomation.Api.Base;
using TestAutomation.Api.TestData;

namespace TestAutomation.Api.Tests;

[Category("Booking")]
public class DeleteBookingTests : ApiTest
{
    private const int NonExistentId = 999999999;

    private int _bookingId;

    [SetUp]
    public async Task CreateBookingToDelete()
    {
        _bookingId = (await CreateBookingAsync(BookingData.NewBooking())).BookingId;
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("DELETE /booking/{id} with a token removes the booking")]
    public async Task DeleteBooking_WithToken_RemovesBooking()
    {
        var response = await BookingClient.DeleteBookingAsync(_bookingId, await GetAdminTokenAsync());

        Assert.That(response.Ok, Is.True, $"Expected success, got {response.Status}");
        Assert.That((await BookingClient.GetBookingAsync(_bookingId)).Status, Is.EqualTo(404));
    }

    [Test]
    [Category("Negative"), Category("Security")]
    [Description("DELETE /booking/{id} without a token is rejected and the booking still exists")]
    public async Task DeleteBooking_WithoutToken_KeepsBooking()
    {
        var response = await BookingClient.DeleteBookingAsync(_bookingId, token: null);

        Assert.That(response.Ok, Is.False, $"Expected a rejection, got {response.Status}");
        Assert.That((await BookingClient.GetBookingAsync(_bookingId)).Status, Is.EqualTo(200));
    }

    // KNOWN BUG: Restful-Booker answers 201 Created to a successful delete.
    [Test]
    [Category("Positive"), Category("KnownBug")]
    [Description("DELETE /booking/{id} returns 200 OK or 204 No Content")]
    public async Task DeleteBooking_ReturnsOkOrNoContent()
    {
        var response = await BookingClient.DeleteBookingAsync(_bookingId, await GetAdminTokenAsync());

        Assert.That(response.Status, Is.EqualTo(200).Or.EqualTo(204));
    }

    // KNOWN BUG: Restful-Booker answers 405 Method Not Allowed for a booking that does not exist.
    [Test]
    [Category("Negative"), Category("KnownBug")]
    [Description("DELETE /booking/{id} for an ID that does not exist returns 404")]
    public async Task DeleteBooking_NonExistentId_Returns404()
    {
        var response = await BookingClient.DeleteBookingAsync(NonExistentId, await GetAdminTokenAsync());

        Assert.That(response.Status, Is.EqualTo(404));
    }
}
