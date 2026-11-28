using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NRZMyk.BrowserTests.Support;
using static Microsoft.Playwright.Assertions;

namespace NRZMyk.BrowserTests.Pages;

/// <summary>
/// Everything specific to signing in: the identity provider's sign-in page (currently Azure AD B2C)
/// and how a signed-in session is kept. The switch to Entra External ID (#244) should only need to change this class.
/// The app signs in through a popup (default login mode of Blazor's MSAL integration).
/// </summary>
public class SignInPage
{
    private static readonly Regex SignInUrl = new(@"^https://[^/]+\.b2clogin\.com/");
    private const float Timeout = 30_000;

    // One sign-in per role and test run. MSAL keeps its tokens in sessionStorage, which Playwright's storage
    // state does not cover, so the session is the identity provider's cookies plus a copy of both web storages.
    private static readonly Dictionary<string, SavedSession> Sessions = new();

    private readonly BrowserSession _browser;
    private readonly TestSettings _settings;
    private Task<IPage>? _popup;

    public SignInPage(BrowserSession browser, TestSettings settings)
    {
        _browser = browser;
        _settings = settings;
    }

    private IPage Page => _browser.Page;

    /// <summary>Call before the action that starts the sign-in, so the popup is not missed.</summary>
    public void ExpectSignIn() => _popup = Page.WaitForPopupAsync(new() { Timeout = Timeout });

    public async Task<IPage> WaitUntilShownAsync()
    {
        var popup = await (_popup ?? throw new InvalidOperationException($"Call {nameof(ExpectSignIn)} first."));
        await popup.WaitForURLAsync(SignInUrl, new() { Timeout = Timeout });
        return popup;
    }

    /// <summary>
    /// Signs in through the sign-in page. The first sign-in of a role is kept for later scenarios;
    /// later sign-ins start their own identity provider session, so signing out there does not affect the kept one.
    /// </summary>
    public async Task SignInAsync(string role)
    {
        var user = _settings.User(role);
        await Page.GotoAsync("/");
        ExpectSignIn();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Anmelden" }).ClickAsync();

        var popup = await WaitUntilShownAsync();
        await popup.Locator("#signInName, #email").First.FillAsync(user.Email);
        await popup.Locator("#password").FillAsync(user.Password);
        var closed = new TaskCompletionSource();
        popup.Close += (_, _) => closed.TrySetResult();
        await popup.Locator("#next").ClickAsync();

        try
        {
            await closed.Task.WaitAsync(TimeSpan.FromMilliseconds(Timeout));
        }
        catch (TimeoutException)
        {
            // Still on the sign-in page: wrong credentials or an extra page (password change, MFA, consent)
            var error = await popup.Locator(".error:visible").AllInnerTextsAsync();
            throw new InvalidOperationException(
                $"Sign-in of {user.Email} did not finish. Sign-in page URL: {popup.Url}. Error: {string.Join(" ", error)}");
        }

        await WaitUntilSignedInAsync();
        if (!Sessions.ContainsKey(role))
        {
            Sessions[role] = await SaveSessionAsync();
        }
    }

    /// <summary>Reuses the kept session of this role, or signs in if there is none yet.</summary>
    public async Task EnsureSignedInAsync(string role)
    {
        if (!Sessions.TryGetValue(role, out var session))
        {
            await SignInAsync(role);
            return;
        }

        await _browser.Context.AddCookiesAsync(session.Cookies);
        await _browser.Context.AddInitScriptAsync(RestoreStorageScript(session));
        await Page.GotoAsync("/");
        await WaitUntilSignedInAsync();
    }

    public async Task SignOutAsync()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Abmelden" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Anmelden" })).ToBeVisibleAsync(new() { Timeout = Timeout });
    }

    // Shown only after the app has resolved the roles, so later steps see the final menu
    private Task WaitUntilSignedInAsync() =>
        Expect(Page.GetByText("Angemeldet als")).ToBeVisibleAsync(new() { Timeout = Timeout });

    private async Task<SavedSession> SaveSessionAsync()
    {
        var cookies = await _browser.Context.CookiesAsync();
        var storage = await Page.EvaluateAsync<string>(
            "() => JSON.stringify({ local: Object.entries(localStorage), session: Object.entries(sessionStorage) })");
        return new SavedSession(
            cookies.Select(c => new Cookie
            {
                Name = c.Name, Value = c.Value, Domain = c.Domain, Path = c.Path, Expires = c.Expires,
                HttpOnly = c.HttpOnly, Secure = c.Secure, SameSite = c.SameSite
            }).ToList(),
            new Uri(Page.Url).GetLeftPart(UriPartial.Authority),
            storage);
    }

    // Restores the storages once per tab, before the app starts, and only on the app's own origin
    private static string RestoreStorageScript(SavedSession session) => $$"""
        (() => {
          if (location.origin !== {{JsonSerializer.Serialize(session.Origin)}} || sessionStorage.getItem('browsertests-restored')) return;
          const storage = {{session.Storage}};
          for (const [key, value] of storage.local) localStorage.setItem(key, value);
          for (const [key, value] of storage.session) sessionStorage.setItem(key, value);
          sessionStorage.setItem('browsertests-restored', '1');
        })();
        """;

    private sealed record SavedSession(List<Cookie> Cookies, string Origin, string Storage);
}
