using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace NRZMyk.BrowserTests.Pages;

/// <summary>
/// Everything specific to the identity provider's sign-in page (currently Azure AD B2C).
/// The switch to Entra External ID (#244) should only need to change this class.
/// The app signs in through a popup (default login mode of Blazor's MSAL integration).
/// </summary>
public class SignInPage
{
    private static readonly Regex SignInUrl = new(@"^https://[^/]+\.b2clogin\.com/");

    private readonly IPage _page;
    private Task<IPage>? _popup;

    public SignInPage(IPage page)
    {
        _page = page;
    }

    /// <summary>Call before the action that starts the sign-in, so the popup is not missed.</summary>
    public void ExpectSignIn() => _popup = _page.WaitForPopupAsync(new() { Timeout = 30_000 });

    public async Task WaitUntilShownAsync()
    {
        var popup = await (_popup ?? throw new InvalidOperationException($"Call {nameof(ExpectSignIn)} first."));
        await popup.WaitForURLAsync(SignInUrl, new() { Timeout = 30_000 });
    }
}
