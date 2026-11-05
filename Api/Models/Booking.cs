using System.Text.Json.Serialization;

namespace TestAutomation.Api.Models;

// The API uses all-lowercase field names, so every property is mapped explicitly.
public record Booking(
    [property: JsonPropertyName("firstname")] string FirstName,
    [property: JsonPropertyName("lastname")] string LastName,
    [property: JsonPropertyName("totalprice")] int TotalPrice,
    [property: JsonPropertyName("depositpaid")] bool DepositPaid,
    [property: JsonPropertyName("bookingdates")] BookingDates BookingDates,
    [property: JsonPropertyName("additionalneeds")] string? AdditionalNeeds = null);

public record BookingDates(
    [property: JsonPropertyName("checkin")] DateOnly CheckIn,
    [property: JsonPropertyName("checkout")] DateOnly CheckOut);

public record CreatedBooking(
    [property: JsonPropertyName("bookingid")] int BookingId,
    [property: JsonPropertyName("booking")] Booking Booking);

public record BookingId([property: JsonPropertyName("bookingid")] int Id);
