using Microsoft.Playwright;
using NRZMyk.BrowserTests.Pages;
using Reqnroll;

namespace NRZMyk.BrowserTests.Steps;

[Binding]
public class AnmeldungSteps
{
    private readonly IPage _page;
    private readonly SignInPage _signInPage;

    public AnmeldungSteps(IPage page, SignInPage signInPage)
    {
        _page = page;
        _signInPage = signInPage;
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
}
