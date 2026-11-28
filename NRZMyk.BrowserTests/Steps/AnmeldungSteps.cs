using Microsoft.Playwright;
using NRZMyk.BrowserTests.Pages;
using NRZMyk.BrowserTests.Support;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace NRZMyk.BrowserTests.Steps;

[Binding]
public class AnmeldungSteps
{
    private readonly BrowserSession _browser;
    private readonly SignInPage _signInPage;
    private readonly TestSettings _settings;

    public AnmeldungSteps(BrowserSession browser, SignInPage signInPage, TestSettings settings)
    {
        _browser = browser;
        _signInPage = signInPage;
        _settings = settings;
    }

    private IPage Page => _browser.Page;

    [Given(@"ich bin als {string} angemeldet")]
    public Task AngenommenIchBinAlsAngemeldet(string role) => _signInPage.EnsureSignedInAsync(role);

    [Given(@"der Testnutzer {string} hat sich noch nie angemeldet")]
    public Task AngenommenDerTestnutzerHatSichNochNieAngemeldet(string role) =>
        TestDatabase.RemoveAccountAsync(_settings, role);

    [When(@"ich ohne Anmeldung die Seite {string} öffne")]
    public async Task WennIchOhneAnmeldungDieSeiteOeffne(string path)
    {
        _signInPage.ExpectSignIn();
        await Page.GotoAsync(path);
    }

    [Then(@"werde ich zur Anmeldeseite weitergeleitet")]
    public async Task DannWerdeIchZurAnmeldeseiteWeitergeleitet()
    {
        await _signInPage.WaitUntilShownAsync();
    }

    [When(@"ich mich als {string} anmelde")]
    public Task WennIchMichAlsAnmelde(string role) => _signInPage.SignInAsync(role);

    [When(@"ich in einem neuen Browserfenster als {string} angemeldet bin")]
    public async Task WennIchInEinemNeuenBrowserfensterAlsAngemeldetBin(string role)
    {
        await _browser.OpenAsync();
        await _signInPage.EnsureSignedInAsync(role);
    }

    [When(@"ich mich abmelde")]
    public Task WennIchMichAbmelde() => _signInPage.SignOutAsync();

    [Then(@"bin ich angemeldet")]
    public async Task DannBinIchAngemeldet()
    {
        await Expect(Page.GetByText("Angemeldet als")).ToBeVisibleAsync();
    }

    [Then(@"bin ich abgemeldet")]
    public async Task DannBinIchAbgemeldet()
    {
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Anmelden" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Angemeldet als")).ToHaveCountAsync(0);
    }

    [Then(@"sehe ich im Menü {string}")]
    public async Task DannSeheIchImMenue(string item)
    {
        await Expect(MenuItem(item)).ToBeVisibleAsync();
    }

    [Then(@"nicht {string}")]
    [Then(@"sehe ich im Menü nicht {string}")]
    public async Task DannSeheIchImMenueNicht(string item)
    {
        await Expect(MenuItem(item)).ToHaveCountAsync(0);
    }

    private ILocator MenuItem(string name) =>
        Page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = name });
}
