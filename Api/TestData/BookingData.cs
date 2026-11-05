using TestAutomation.Api.Models;

namespace TestAutomation.Api.TestData;

public static class BookingData
{
    // The first name is unique so a test can find its own booking among everyone else's on the shared API.
    public static Booking NewBooking() => new(
        FirstName: "Test" + Guid.NewGuid().ToString("N")[..8],
        LastName: "Booker",
        TotalPrice: 150,
        DepositPaid: true,
        BookingDates: new(new DateOnly(2027, 1, 10), new DateOnly(2027, 1, 14)),
        AdditionalNeeds: "Breakfast");

    public static Booking UpdatedBooking() => new(
        FirstName: "Updated" + Guid.NewGuid().ToString("N")[..8],
        LastName: "Changed",
        TotalPrice: 320,
        DepositPaid: false,
        BookingDates: new(new DateOnly(2027, 3, 1), new DateOnly(2027, 3, 5)),
        AdditionalNeeds: "Late checkout");
}
