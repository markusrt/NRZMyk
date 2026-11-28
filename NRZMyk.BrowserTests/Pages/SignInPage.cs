using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NRZMyk.BrowserTests.Support;

namespace NRZMyk.BrowserTests.Pages;

/// <summary>
/// Everything specific to the identity provider's sign-in page (currently Azure AD B2C).
/// The switch to Entra External ID (#244) should only need to change this class.
/// The app signs in through a popup (default login mode of Blazor's MSAL integration).
/// </summary>
public class SignInPage
{
    private static readonly Regex SignInUrl = new(@"^https://[^/]+\.b2clogin\.com/");
    private const float Timeout = 30_000;

    private readonly IPage _page;
    private Task<IPage>? _popup;

    public SignInPage(IPage page)
    {
        _page = page;
    }

    /// <summary>Call before the action that starts the sign-in, so the popup is not missed.</summary>
    public void ExpectSignIn() => _popup = _page.WaitForPopupAsync(new() { Timeout = Timeout });

    public async Task<IPage> WaitUntilShownAsync()
    {
        var popup = await (_popup ?? throw new InvalidOperationException($"Call {nameof(ExpectSignIn)} first."));
        await popup.WaitForURLAsync(SignInUrl, new() { Timeout = Timeout });
        return popup;
    }

    public async Task SignInAsync(TestUser user)
    {
        await _page.GotoAsync("/");
        ExpectSignIn();
        await _page.GetByRole(AriaRole.Link, new() { Name = "Anmelden" }).ClickAsync();

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
    }
}
