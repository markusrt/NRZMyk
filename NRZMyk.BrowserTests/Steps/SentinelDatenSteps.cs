using FluentAssertions;
using Microsoft.Playwright;
using NRZMyk.BrowserTests.Support;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace NRZMyk.BrowserTests.Steps;

/// <summary>Steps for the list of sentinel entries, the entry form, the cryo view and the export.</summary>
[Binding]
public class SentinelDatenSteps
{
    private readonly BrowserSession _browser;
    private readonly TestSettings _settings;
    private IDownload? _download;

    public SentinelDatenSteps(BrowserSession browser, TestSettings settings)
    {
        _browser = browser;
        _settings = settings;
    }

    private IPage Page => _browser.Page;

    private ILocator Rows => Page.Locator("table tbody tr");

    private ILocator Row(string text) => Rows.Filter(new() { HasText = text });

    [When(@"ich nach {string} suche")]
    public async Task WennIchNachSuche(string term)
    {
        await Page.Locator("#search-input").FillAsync(term);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Suchen", Exact = true }).ClickAsync();
        await Expect(Page.GetByText($"Suchergebnisse für \"{term}\"")).ToBeVisibleAsync();
    }

    [Then(@"sehe ich {int} Einträge")]
    [Then(@"sehe ich {int} Eintrag")]
    public async Task DannSeheIchEintraege(int count)
    {
        await Expect(Rows).ToHaveCountAsync(count);
    }

    [Then(@"sehe ich den Eintrag {string}")]
    public async Task DannSeheIchDenEintrag(string text)
    {
        await Expect(Row(text)).ToHaveCountAsync(1);
    }

    [When(@"ich beim Eintrag {string} auf {string} klicke")]
    public async Task WennIchBeimEintragAufKlicke(string text, string action)
    {
        // The row actions are icon links named by their title
        await Row(text).Locator($"a[title='{action}']").ClickAsync();
    }

    [When(@"ich einen MHK-Eintrag {string} für {string} mit dem Wert {string} hinzufüge")]
    public async Task WennIchEinenMhkEintragHinzufuege(string testingMethod, string antifungalAgent, string value)
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

    [Then(@"wird der MHK-Eintrag als {string} bewertet")]
    public async Task DannWirdDerMhkEintragBewertet(string resistance)
    {
        await Expect(Page.Locator("span.badge").Filter(new() { HasText = resistance })).ToHaveCountAsync(1);
    }

    [When(@"ich in der Kryo-Ansicht die Einsendungen von {string} lade")]
    public async Task WennIchInDerKryoAnsichtDieEinsendungenLade(string organization)
    {
        await Page.GotoAsync("cryo-view/sentinel-entries");
        var select = Page.Locator("xpath=//label[normalize-space()='Organisation']/following-sibling::div[1]//select");
        await select.SelectOptionAsync(new SelectOptionValue { Label = organization });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Einsendungen laden" }).ClickAsync();
        await Expect(Rows.First).ToBeVisibleAsync();
    }

    [Then(@"ist der Eintrag {string} heute asserviert")]
    public async Task DannIstDerEintragHeuteAsserviert(string laboratoryNumber)
    {
        await Expect(Row(laboratoryNumber)).ToContainTextAsync(DateTime.Today.ToString("dd.MM.yyyy"));
        await Expect(Row(laboratoryNumber).Locator("a[title='Aus Cryobox entfernen']")).ToBeVisibleAsync();
    }

    [When(@"ich beim Eintrag {string} die zusätzliche Info {string} speichere")]
    public async Task WennIchDieZusaetzlicheInfoSpeichere(string laboratoryNumber, string remark)
    {
        var input = Page.GetByLabel($"Zusätzliche Informationen für {laboratoryNumber}");
        await input.FillAsync(remark);
        await Row(laboratoryNumber).Locator("a[title='Zusätzliche Info aktualisieren']").ClickAsync();
        await Expect(Page.GetByLabel($"Zusätzliche Informationen für {laboratoryNumber}")).ToHaveValueAsync(remark);
    }

    [Then(@"hat der Eintrag {string} die zusätzliche Info {string}")]
    public async Task DannHatDerEintragDieZusaetzlicheInfo(string laboratoryNumber, string remark)
    {
        await Expect(Page.GetByLabel($"Zusätzliche Informationen für {laboratoryNumber}")).ToHaveValueAsync(remark);
    }

    [When(@"ich den Excel-Export starte")]
    public async Task WennIchDenExcelExportStarte()
    {
        _download = await Page.RunAndWaitForDownloadAsync(
            () => Page.GetByRole(AriaRole.Button, new() { Name = "Export" }).ClickAsync(),
            new() { Timeout = 120_000 });
    }

    [Then(@"wird eine Excel-Datei heruntergeladen")]
    public async Task DannWirdEineExcelDateiHeruntergeladen()
    {
        var download = _download ?? throw new InvalidOperationException("No export was started.");
        download.SuggestedFilename.Should().MatchRegex(@"^Sentinel-Export_\d{8}\.xlsx$");
        var bytes = await File.ReadAllBytesAsync(await download.PathAsync());
        // An .xlsx file is a zip archive, which starts with "PK"
        bytes.Length.Should().BeGreaterThan(1000);
        bytes[..2].Should().Equal("PK"u8.ToArray());
    }

    [When(@"ich dem Testnutzer {string} die Organisation {string} zuordne")]
    public async Task WennIchDemTestnutzerDieOrganisationZuordne(string role, string organization)
    {
        var row = Row(_settings.User(role).Email);
        await row.Locator("select").SelectOptionAsync(new SelectOptionValue { Label = organization });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Änderungen speichern" }).ClickAsync();
        await Expect(Page.GetByText("Änderungen wurden gespeichert")).ToBeVisibleAsync();
    }

    [Then(@"ist dem Testnutzer {string} die Organisation {string} zugeordnet")]
    public async Task DannIstDemTestnutzerDieOrganisationZugeordnet(string role, string organization)
    {
        var row = Row(_settings.User(role).Email);
        await Expect(row.Locator("select option:checked")).ToHaveTextAsync(organization);
    }

    [Then(@"zeigt die Benutzerliste folgende Rollen:")]
    public async Task DannZeigtDieBenutzerlisteFolgendeRollen(Table table)
    {
        foreach (var expected in table.Rows)
        {
            var row = Row(_settings.User(expected["Testnutzer"]).Email);
            await Expect(row.Locator("td").Nth(3)).ToHaveTextAsync(expected["Rolle"]);
        }
    }
}
