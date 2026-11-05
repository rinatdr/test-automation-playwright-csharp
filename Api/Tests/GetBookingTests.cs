using TestAutomation.Api.Base;
using TestAutomation.Api.Models;
using TestAutomation.Api.TestData;

namespace TestAutomation.Api.Tests;

[Category("Booking")]
public class GetBookingTests : ApiTest
{
    private const int NonExistentId = 999999999;

    [Test]
    [Category("Positive")]
    [Description("GET /booking returns the IDs of all bookings, including a newly created one")]
    public async Task GetBookingIds_IncludesCreatedBooking()
    {
        var created = await CreateBookingAsync(BookingData.NewBooking());

        var response = await BookingClient.GetBookingIdsAsync();

        Assert.That(response.Status, Is.EqualTo(200));
        var ids = await response.ReadJsonAsync<BookingId[]>();
        Assert.That(ids.Select(b => b.Id), Does.Contain(created.BookingId));
    }

    [Test]
    [Category("Positive")]
    [Description("GET /booking filtered by first and last name returns only the matching booking")]
    public async Task GetBookingIds_FilteredByName_ReturnsOnlyMatchingBooking()
    {
        var booking = BookingData.NewBooking();
        var created = await CreateBookingAsync(booking);

        var response = await BookingClient.GetBookingIdsAsync(booking.FirstName, booking.LastName);

        Assert.That(response.Status, Is.EqualTo(200));
        var ids = await response.ReadJsonAsync<BookingId[]>();
        Assert.That(ids.Select(b => b.Id), Is.EqualTo(new[] { created.BookingId }));
    }

    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("GET /booking/{id} returns every detail of the booking")]
    public async Task GetBooking_ReturnsAllDetails()
    {
        var booking = BookingData.NewBooking();
        var created = await CreateBookingAsync(booking);

        var response = await BookingClient.GetBookingAsync(created.BookingId);

        Assert.That(response.Status, Is.EqualTo(200));
        Assert.That(await response.ReadJsonAsync<Booking>(), Is.EqualTo(booking));
    }

    [Test]
    [Category("Negative")]
    [Description("GET /booking/{id} for an ID that does not exist returns 404")]
    public async Task GetBooking_NonExistentId_Returns404()
    {
        var response = await BookingClient.GetBookingAsync(NonExistentId);

        Assert.That(response.Status, Is.EqualTo(404));
    }
}
