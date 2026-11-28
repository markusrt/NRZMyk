using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace NRZMyk.BrowserTests.Support;

/// <summary>
/// The browser window of a scenario. A scenario that switches users opens a new window,
/// so the sessions of two users never mix.
/// </summary>
public sealed class BrowserSession
{
    // Application Insights is configured in index.html; its requests must not leave the test machine
    private static readonly Regex TelemetryHosts = new(
        @"^https://([^/]*\.)?(vo\.msecnd\.net|monitor\.azure\.com|applicationinsights\.azure\.com|services\.visualstudio\.com)/");

    private readonly IBrowser _browser;
    private readonly TestSettings _settings;

    public BrowserSession(IBrowser browser, TestSettings settings)
    {
        _browser = browser;
        _settings = settings;
    }

    public IBrowserContext Context { get; private set; } = null!;

    public IPage Page { get; private set; } = null!;

    /// <summary>Opens a new window; the previous one is closed.</summary>
    public async Task OpenAsync()
    {
        await CloseAsync();

        Context = await _browser.NewContextAsync(new()
        {
            BaseURL = _settings.BaseUrl,
            Locale = "de-DE",
            AcceptDownloads = true
        });
        await Context.RouteAsync(TelemetryHosts, route => route.AbortAsync());
        await Context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true, Sources = true });
        Page = await Context.NewPageAsync();
    }

    /// <summary>Closes the window, keeping a screenshot and trace in <paramref name="failureDirectory"/> if given.</summary>
    public async Task CloseAsync(string? failureDirectory = null)
    {
        if (Context == null)
        {
            return;
        }

        if (failureDirectory != null)
        {
            Directory.CreateDirectory(failureDirectory);
            var screenshot = Path.Combine(failureDirectory, "screenshot.png");
            var trace = Path.Combine(failureDirectory, "trace.zip");
            await Page.ScreenshotAsync(new() { Path = screenshot, FullPage = true });
            await Context.Tracing.StopAsync(new() { Path = trace });
            NUnit.Framework.TestContext.AddTestAttachment(screenshot);
            NUnit.Framework.TestContext.AddTestAttachment(trace);
        }
        else
        {
            await Context.Tracing.StopAsync();
        }

        await Context.DisposeAsync();
        Context = null!;
        Page = null!;
    }
}
