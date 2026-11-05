namespace TestAutomation.Api.Models;

public record Credentials(string Username, string Password);

// On success the API returns a token; on failure it returns a reason instead.
public record AuthResponse(string? Token, string? Reason);
