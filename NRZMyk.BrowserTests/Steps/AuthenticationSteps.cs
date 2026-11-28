using Microsoft.Playwright;
using NRZMyk.BrowserTests.Pages;
using NRZMyk.BrowserTests.Support;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace NRZMyk.BrowserTests.Steps;

[Binding]
public class AuthenticationSteps
{
    private readonly BrowserSession _browser;
    private readonly SignInPage _signInPage;
    private readonly TestSettings _settings;

    public AuthenticationSteps(BrowserSession browser, SignInPage signInPage, TestSettings settings)
    {
        _browser = browser;
        _signInPage = signInPage;
        _settings = settings;
    }

    private IPage Page => _browser.Page;

    [Given(@"I am signed in as {string}")]
    public Task GivenIAmSignedInAs(string role) => _signInPage.EnsureSignedInAsync(role);

    [Given(@"the test user {string} has never signed in")]
    public Task GivenTheTestUserHasNeverSignedIn(string role) =>
        TestDatabase.RemoveAccountAsync(_settings, role);

    [When(@"I open the page {string} without signing in")]
    public async Task WhenIOpenThePageWithoutSigningIn(string path)
    {
        _signInPage.ExpectSignIn();
        await Page.GotoAsync(path);
    }

    [Then(@"I am redirected to the sign-in page")]
    public async Task ThenIAmRedirectedToTheSignInPage()
    {
        await _signInPage.WaitUntilShownAsync();
    }

    [When(@"I sign in as {string}")]
    public Task WhenISignInAs(string role) => _signInPage.SignInAsync(role);

    [When(@"I am signed in as {string} in a new browser window")]
    public async Task WhenIAmSignedInAsInANewBrowserWindow(string role)
    {
        await _browser.OpenAsync();
        await _signInPage.EnsureSignedInAsync(role);
    }

    [When(@"I sign out")]
    public Task WhenISignOut() => _signInPage.SignOutAsync();

    [Then(@"I am signed in")]
    public async Task ThenIAmSignedIn()
    {
        await Expect(Page.GetByText("Angemeldet als")).ToBeVisibleAsync();
    }

    [Then(@"I am signed out")]
    public async Task ThenIAmSignedOut()
    {
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Anmelden" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Angemeldet als")).ToHaveCountAsync(0);
    }

    [Then(@"the menu shows {string}")]
    public async Task ThenTheMenuShows(string item)
    {
        await Expect(MenuItem(item)).ToBeVisibleAsync();
    }

    [Then(@"the menu does not show {string}")]
    public async Task ThenTheMenuDoesNotShow(string item)
    {
        await Expect(MenuItem(item)).ToHaveCountAsync(0);
    }

    private ILocator MenuItem(string name) =>
        Page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = name });
}
