using TestAutomation.Api.Base;
using TestAutomation.Api.Models;
using TestAutomation.Api.TestData;

namespace TestAutomation.Api.Tests;

[Category("Booking")]
public class CreateBookingTests : ApiTest
{
    [Test]
    [Category("Positive"), Category("Smoke")]
    [Description("POST /booking without a token creates a booking and returns its ID and details")]
    public async Task CreateBooking_ReturnsNewIdAndSameDetails()
    {
        var booking = BookingData.NewBooking();

        var response = await PostBookingAsync(booking);

        Assert.That(response.Status, Is.EqualTo(200));
        var created = await response.ReadJsonAsync<CreatedBooking>();
        Assert.That(created.BookingId, Is.Positive);
        Assert.That(created.Booking, Is.EqualTo(booking));
    }

    [Test]
    [Category("Positive")]
    [Description("POST /booking without the optional additional needs creates the booking")]
    public async Task CreateBooking_WithoutAdditionalNeeds_CreatesBooking()
    {
        var booking = BookingData.NewBooking() with { AdditionalNeeds = null };

        var created = await CreateBookingAsync(booking);

        var stored = await (await BookingClient.GetBookingAsync(created.BookingId)).ReadJsonAsync<Booking>();
        Assert.That(stored, Is.EqualTo(booking));
    }

    // KNOWN BUG: Restful-Booker crashes with 500 Internal Server Error instead of rejecting the request.
    [Test]
    [Category("Negative"), Category("KnownBug")]
    [Description("POST /booking with missing required fields returns 400 Bad Request")]
    public async Task CreateBooking_WithMissingRequiredFields_Returns400()
    {
        var response = await PostBookingAsync(new { firstname = "OnlyFirstName" });

        Assert.That(response.Status, Is.EqualTo(400));
    }

    // KNOWN BUG: Restful-Booker accepts a checkout date that is before the checkin date.
    [Test]
    [Category("Negative"), Category("KnownBug")]
    [Description("POST /booking with checkout before checkin returns 400 Bad Request")]
    public async Task CreateBooking_WithCheckoutBeforeCheckin_Returns400()
    {
        var booking = BookingData.NewBooking() with
        {
            BookingDates = new(new DateOnly(2027, 1, 14), new DateOnly(2027, 1, 10))
        };

        var response = await PostBookingAsync(booking);

        Assert.That(response.Status, Is.EqualTo(400));
    }

    // KNOWN BUG: Restful-Booker accepts a negative total price.
    [Test]
    [Category("Negative"), Category("KnownBug")]
    [Description("POST /booking with a negative total price returns 400 Bad Request")]
    public async Task CreateBooking_WithNegativePrice_Returns400()
    {
        var booking = BookingData.NewBooking() with { TotalPrice = -50 };

        var response = await PostBookingAsync(booking);

        Assert.That(response.Status, Is.EqualTo(400));
    }
}
