using FluentAssertions;
using Microsoft.Playwright;
using NRZMyk.BrowserTests.Support;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace NRZMyk.BrowserTests.Steps;

/// <summary>Steps for the list of sentinel entries, the entry form, the cryo view, the export and the user list.</summary>
[Binding]
public class SentinelEntrySteps
{
    private readonly BrowserSession _browser;
    private readonly TestSettings _settings;
    private IDownload? _download;

    public SentinelEntrySteps(BrowserSession browser, TestSettings settings)
    {
        _browser = browser;
        _settings = settings;
    }

    private IPage Page => _browser.Page;

    private ILocator Rows => Page.Locator("table tbody tr");

    private ILocator Row(string text) => Rows.Filter(new() { HasText = text });

    [When(@"I search for {string}")]
    public async Task WhenISearchFor(string term)
    {
        await Page.Locator("#search-input").FillAsync(term);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Suchen", Exact = true }).ClickAsync();
        await Expect(Page.GetByText($"Suchergebnisse für \"{term}\"")).ToBeVisibleAsync();
    }

    [Then(@"I see {int} entries")]
    [Then(@"I see {int} entry")]
    public async Task ThenISeeEntries(int count)
    {
        await Expect(Rows).ToHaveCountAsync(count);
    }

    [When(@"I click {string} for the entry {string}")]
    public async Task WhenIClickForTheEntry(string action, string text)
    {
        // The row actions are icon links named by their title
        await Row(text).Locator($"a[title='{action}']").ClickAsync();
    }

    [When(@"I add an MIC entry {string} for {string} with the value {string}")]
    public async Task WhenIAddAnMicEntry(string testingMethod, string antifungalAgent, string value)
    {
        var add = Page.Locator("#addAntimicrobialSensitivityTest");
        var selection = add.Locator("xpath=ancestor::div[contains(@class,'row')][1]").Locator("select");
        await selection.Nth(0).SelectOptionAsync(new SelectOptionValue { Label = testingMethod });
        await selection.Nth(1).SelectOptionAsync(new SelectOptionValue { Label = antifungalAgent });
        await add.ClickAsync();

        var mic = Page.Locator("div.row")
            .Filter(new() { Has = Page.Locator(".input-group-text", new() { HasText = "MHK" }) })
            .Filter(new() { HasText = $"{testingMethod} - {antifungalAgent}" })
            .Locator(".input-group select");
        await mic.SelectOptionAsync(new SelectOptionValue { Label = value });
    }

    [Then(@"the MIC entry is rated {string}")]
    public async Task ThenTheMicEntryIsRated(string resistance)
    {
        await Expect(Page.Locator("span.badge").Filter(new() { HasText = resistance })).ToHaveCountAsync(1);
    }

    [When(@"I load the submissions of {string} in the cryo view")]
    public async Task WhenILoadTheSubmissionsInTheCryoView(string organization)
    {
        await Page.GotoAsync("cryo-view/sentinel-entries");
        var select = Page.Locator("xpath=//label[normalize-space()='Organisation']/following-sibling::div[1]//select");
        await select.SelectOptionAsync(new SelectOptionValue { Label = organization });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Einsendungen laden" }).ClickAsync();
        await Expect(Rows.First).ToBeVisibleAsync();
    }

    [Then(@"the entry {string} is stored in a cryo box today")]
    public async Task ThenTheEntryIsStoredInACryoBoxToday(string laboratoryNumber)
    {
        await Expect(Row(laboratoryNumber)).ToContainTextAsync(DateTime.Today.ToString("dd.MM.yyyy"));
        await Expect(Row(laboratoryNumber).Locator("a[title='Aus Cryobox entfernen']")).ToBeVisibleAsync();
    }

    [When(@"I save the additional info {string} for the entry {string}")]
    public async Task WhenISaveTheAdditionalInfo(string remark, string laboratoryNumber)
    {
        var input = AdditionalInfo(laboratoryNumber);
        await input.FillAsync(remark);
        await Row(laboratoryNumber).Locator("a[title='Zusätzliche Info aktualisieren']").ClickAsync();
        await Expect(AdditionalInfo(laboratoryNumber)).ToHaveValueAsync(remark);
    }

    [Then(@"the entry {string} has the additional info {string}")]
    public async Task ThenTheEntryHasTheAdditionalInfo(string laboratoryNumber, string remark)
    {
        await Expect(AdditionalInfo(laboratoryNumber)).ToHaveValueAsync(remark);
    }

    private ILocator AdditionalInfo(string laboratoryNumber) =>
        Page.GetByLabel($"Zusätzliche Informationen für {laboratoryNumber}");

    [When(@"I start the Excel export")]
    public async Task WhenIStartTheExcelExport()
    {
        _download = await Page.RunAndWaitForDownloadAsync(
            () => Page.GetByRole(AriaRole.Button, new() { Name = "Export" }).ClickAsync(),
            new() { Timeout = 120_000 });
    }

    [Then(@"an Excel file is downloaded")]
    public async Task ThenAnExcelFileIsDownloaded()
    {
        var download = _download ?? throw new InvalidOperationException("No export was started.");
        download.SuggestedFilename.Should().MatchRegex(@"^Sentinel-Export_\d{8}\.xlsx$");
        var bytes = await File.ReadAllBytesAsync(await download.PathAsync());
        // An .xlsx file is a zip archive, which starts with "PK"
        bytes.Length.Should().BeGreaterThan(1000);
        bytes[..2].Should().Equal("PK"u8.ToArray());
    }

    [When(@"I assign the organization {string} to the test user {string}")]
    public async Task WhenIAssignTheOrganizationToTheTestUser(string organization, string role)
    {
        var row = Row(_settings.User(role).Email);
        await row.Locator("select").SelectOptionAsync(new SelectOptionValue { Label = organization });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Änderungen speichern" }).ClickAsync();
        await Expect(Page.GetByText("Änderungen wurden gespeichert")).ToBeVisibleAsync();
    }

    [Then(@"the test user {string} is assigned the organization {string}")]
    public async Task ThenTheTestUserIsAssignedTheOrganization(string role, string organization)
    {
        var row = Row(_settings.User(role).Email);
        await Expect(row.Locator("select option:checked")).ToHaveTextAsync(organization);
    }

    [Then(@"the user list shows the following roles:")]
    public async Task ThenTheUserListShowsTheFollowingRoles(Table table)
    {
        foreach (var expected in table.Rows)
        {
            var row = Row(_settings.User(expected["Test user"]).Email);
            await Expect(row.Locator("td").Nth(3)).ToHaveTextAsync(expected["Role"]);
        }
    }
}
