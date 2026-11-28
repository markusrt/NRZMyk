using Microsoft.Playwright;
using NRZMyk.BrowserTests.Pages;
using NRZMyk.BrowserTests.Support;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace NRZMyk.BrowserTests.Steps;

[Binding]
public class AnmeldungSteps
{
    private readonly IPage _page;
    private readonly SignInPage _signInPage;
    private readonly TestSettings _settings;

    public AnmeldungSteps(IPage page, SignInPage signInPage, TestSettings settings)
    {
        _page = page;
        _signInPage = signInPage;
        _settings = settings;
    }

    [When(@"ich ohne Anmeldung die Seite {string} öffne")]
    public async Task WennIchOhneAnmeldungDieSeiteOeffne(string path)
    {
        _signInPage.ExpectSignIn();
        await _page.GotoAsync(path);
    }

    [Then(@"werde ich zur Anmeldeseite weitergeleitet")]
    public async Task DannWerdeIchZurAnmeldeseiteWeitergeleitet()
    {
        await _signInPage.WaitUntilShownAsync();
    }

    [When(@"ich mich als {string} anmelde")]
    public async Task WennIchMichAlsAnmelde(string role)
    {
        await _signInPage.SignInAsync(_settings.User(role));
    }

    [Then(@"bin ich angemeldet")]
    public async Task DannBinIchAngemeldet()
    {
        // Shown only after the app has resolved the roles, so the menu checks below see the final state
        await Expect(_page.GetByText("Angemeldet als")).ToBeVisibleAsync(new() { Timeout = 30_000 });
    }

    [Then(@"sehe ich im Menü {string}")]
    public async Task DannSeheIchImMenue(string item)
    {
        await Expect(MenuItem(item)).ToBeVisibleAsync();
    }

    [Then(@"nicht {string}")]
    public async Task DannNicht(string item)
    {
        await Expect(MenuItem(item)).ToHaveCountAsync(0);
    }

    private ILocator MenuItem(string name) =>
        _page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = name });
}
