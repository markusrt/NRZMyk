using Microsoft.EntityFrameworkCore;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.BrowserTests.Support;

/// <summary>
/// Resets the test database to the same deterministic test data before each scenario,
/// so scenarios do not depend on each other or on their order.
/// The schema is created by the app itself, which applies the EF migrations on startup.
/// Clinical breakpoints are seeded by the app from its configuration and are kept.
/// </summary>
public static class TestDatabase
{
    public const string MainOrganization = "Nationales Referenzzentrum für Invasive Pilzinfektionen";
    public const string UserOrganization = "Testlabor Nord";
    public const string OtherOrganization = "Testlabor Süd";

    // Laboratory numbers SN-2025-0001 to SN-2025-0055: sender numbers NORD-0001 to NORD-0055 (more than one page of 50)
    public const int UserOrganizationEntries = 55;
    // SN-2025-0056: SUCHE-4711, SN-2025-0057: ARCHIV-0001 (already in the cryo box), both of the user's organization
    // SN-2025-0058 to SN-2025-0060: SUED-0001 to SUED-0003 of the other organization
    private const int SeedYear = 2025;

    public static async Task ResetAsync(TestSettings settings)
    {
        await using var context = CreateContext(settings);

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

        for (var number = 1; number <= UserOrganizationEntries; number++)
        {
            context.SentinelEntries.Add(Entry(user, number, $"NORD-{number:0000}"));
        }
        context.SentinelEntries.Add(Entry(user, 56, "SUCHE-4711"));
        context.SentinelEntries.Add(Entry(user, 57, "ARCHIV-0001", cryoDate: new DateTime(2025, 7, 1)));
        for (var number = 1; number <= 3; number++)
        {
            context.SentinelEntries.Add(Entry(other, 57 + number, $"SUED-{number:0000}"));
        }

        await context.SaveChangesAsync();
    }

    /// <summary>Removes the account of a test user, as if the user had never signed in to the app.</summary>
    public static async Task RemoveAccountAsync(TestSettings settings, string role)
    {
        await using var context = CreateContext(settings);
        var objectId = settings.User(role).ObjectId;
        await context.RemoteAccounts.Where(a => a.ObjectId == objectId).ExecuteDeleteAsync();
    }

    private static ApplicationDbContext CreateContext(TestSettings settings) =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(settings.ConnectionString).Options);

    private static SentinelEntry Entry(Organization organization, int number, string senderLaboratoryNumber, DateTime? cryoDate = null) => new()
    {
        Year = SeedYear,
        YearlySequentialEntryNumber = number,
        CryoBoxNumber = 1,
        CryoBoxSlot = number,
        CryoDate = cryoDate,
        ProtectKey = organization.Id.ToString(),
        ReceivingDate = new DateTime(SeedYear, 6, 1),
        SamplingDate = new DateTime(SeedYear, 5, 1).AddDays(number % 28),
        SenderLaboratoryNumber = senderLaboratoryNumber,
        Material = Material.PeripheralBloodCulture,
        HospitalDepartment = HospitalDepartment.GeneralSurgery,
        IdentifiedSpecies = Species.CandidaAlbicans,
        SpeciesIdentificationMethod = SpeciesIdentificationMethod.MaldiTof,
        // Lets an entry without susceptibility tests pass validation when it is edited
        Remark = "Testdaten",
        AntimicrobialSensitivityTests = new List<AntimicrobialSensitivityTest>()
    };

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
