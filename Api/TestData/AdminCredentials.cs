using TestAutomation.Api.Models;

namespace TestAutomation.Api.TestData;

public static class AdminCredentials
{
    public static readonly Credentials Valid = new("admin", "password123");
    public static readonly Credentials WrongPassword = new("admin", "wrong-password");
}
