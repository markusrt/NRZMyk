using Microsoft.EntityFrameworkCore;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.BrowserTests.Support;

/// <summary>
/// Resets the test database to the same deterministic test data before each run.
/// The schema is created by the app itself, which applies the EF migrations on startup.
/// Clinical breakpoints are seeded by the app from its configuration and are kept.
/// </summary>
public static class TestDatabase
{
    public const string MainOrganization = "Nationales Referenzzentrum für Invasive Pilzinfektionen";
    public const string UserOrganization = "Testlabor Nord";
    public const string OtherOrganization = "Testlabor Süd";

    public static async Task ResetAsync(TestSettings settings)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(settings.ConnectionString)
            .Options;
        await using var context = new ApplicationDbContext(options);

        await context.Database.ExecuteSqlRawAsync("""
            UPDATE SentinelEntries SET PredecessorEntryId = NULL;
            DELETE FROM AntimicrobialSensitivityTest;
            DELETE FROM SentinelEntries;
            DELETE FROM RemoteAccounts;
            DELETE FROM Organizations;
            """);

        var main = new Organization { Name = MainOrganization, DispatchMonth = MonthToDispatch.January };
        var user = new Organization { Name = UserOrganization, DispatchMonth = MonthToDispatch.March };
        var other = new Organization { Name = OtherOrganization, DispatchMonth = MonthToDispatch.June };
        context.Organizations.AddRange(main, user, other);

        // Accounts exist before the first sign-in, so the app never treats a test user as new
        AddAccount(context, settings, "Guest", organization: null);
        AddAccount(context, settings, "User", user);
        AddAccount(context, settings, "SuperUser", main);
        AddAccount(context, settings, "Admin", main);

        await context.SaveChangesAsync();
    }

    private static void AddAccount(ApplicationDbContext context, TestSettings settings, string role, Organization? organization)
    {
        if (!settings.Users.TryGetValue(role, out var testUser) || testUser.ObjectId == Guid.Empty)
        {
            return;
        }

        context.RemoteAccounts.Add(new RemoteAccount
        {
            ObjectId = testUser.ObjectId,
            DisplayName = $"Test {role}",
            Email = testUser.Email,
            City = "Teststadt",
            Organization = organization
        });
    }
}
