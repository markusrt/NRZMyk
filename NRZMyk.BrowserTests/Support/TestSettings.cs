using Microsoft.Extensions.Configuration;

namespace NRZMyk.BrowserTests.Support;

/// <summary>
/// Settings from user secrets and environment variables (prefix "BrowserTests__").
/// Credentials and Azure values are never part of the repository.
/// </summary>
public class TestSettings
{
    public string BaseUrl { get; set; } = "http://localhost:5080";

    public string ConnectionString { get; set; } =
        "Server=127.0.0.1,14330;Database=NRZMyk;User Id=sa;Password=BrowserTests#Local1;TrustServerCertificate=True";

    public bool Headed { get; set; }

    public Dictionary<string, TestUser> Users { get; set; } = new();

    public static TestSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TestSettings>(optional: true)
            .AddEnvironmentVariables()
            .Build();
        var settings = new TestSettings();
        configuration.GetSection("BrowserTests").Bind(settings);
        return settings;
    }

    public TestUser User(string role)
    {
        if (!Users.TryGetValue(role, out var user) || !user.IsComplete)
        {
            throw new InvalidOperationException(
                $"Test user for role '{role}' is not configured. Set BrowserTests:Users:{role}:Email, :Password and :ObjectId " +
                "in the user secrets of NRZMyk.BrowserTests (see README, section \"Browser tests\").");
        }
        return user;
    }
}

public class TestUser
{
    public string Email { get; set; } = "";

    public string Password { get; set; } = "";

    public Guid ObjectId { get; set; }

    public bool IsComplete => Email != "" && Password != "" && ObjectId != Guid.Empty;
}
