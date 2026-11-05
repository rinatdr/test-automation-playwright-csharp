namespace TestAutomation.UI.TestData;

public record User(string Username, string Password);

public static class Users
{
    private const string Password = "secret_sauce";

    public static readonly User Standard = new("standard_user", Password);
    public static readonly User LockedOut = new("locked_out_user", Password);
    public static readonly User Problem = new("problem_user", Password);
    public static readonly User PerformanceGlitch = new("performance_glitch_user", Password);
    public static readonly User Error = new("error_user", Password);
    public static readonly User Visual = new("visual_user", Password);

    public static readonly User[] CanLogin = [Standard, Problem, PerformanceGlitch, Error, Visual];
}
