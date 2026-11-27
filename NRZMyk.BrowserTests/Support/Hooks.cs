using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;

namespace NRZMyk.BrowserTests.Support;

[Binding]
public sealed class Hooks
{
    // Application Insights is configured in index.html; its requests must not leave the test machine
    private static readonly Regex TelemetryHosts = new(
        @"^https://([^/]*\.)?(vo\.msecnd\.net|monitor\.azure\.com|applicationinsights\.azure\.com|services\.visualstudio\.com)/");

    private static readonly string ResultsDirectory = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "TestResults");

    private static IPlaywright? _playwright;
    private static IBrowser? _browser;

    private readonly IObjectContainer _container;
    private readonly ScenarioContext _scenario;
    private IBrowserContext? _context;
    private IPage? _page;

    public Hooks(IObjectContainer container, ScenarioContext scenario)
    {
        _container = container;
        _scenario = scenario;
    }

    [BeforeTestRun]
    public static async Task BeforeTestRun(IObjectContainer testThreadContainer)
    {
        var settings = TestSettings.Load();
        testThreadContainer.RegisterInstanceAs(settings);

        await WaitForAppAsync(settings.BaseUrl);
        await TestDatabase.ResetAsync(settings);

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = !settings.Headed });
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        if (_browser != null)
        {
            await _browser.DisposeAsync();
        }
        _playwright?.Dispose();
    }

    [BeforeScenario]
    public async Task BeforeScenario(TestSettings settings)
    {
        _context = await _browser!.NewContextAsync(new()
        {
            BaseURL = settings.BaseUrl,
            Locale = "de-DE",
            AcceptDownloads = true
        });
        await _context.RouteAsync(TelemetryHosts, route => route.AbortAsync());
        await _context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true, Sources = true });

        _page = await _context.NewPageAsync();
        _container.RegisterInstanceAs(_context);
        _container.RegisterInstanceAs(_page);
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_context == null || _page == null)
        {
            return;
        }

        if (_scenario.TestError != null)
        {
            var name = Regex.Replace(_scenario.ScenarioInfo.Title, @"[^\w\-]+", "_");
            var directory = Path.GetFullPath(Path.Combine(ResultsDirectory, $"{DateTime.Now:yyyyMMdd-HHmmss}_{name}"));
            Directory.CreateDirectory(directory);
            await _page.ScreenshotAsync(new() { Path = Path.Combine(directory, "screenshot.png"), FullPage = true });
            await _context.Tracing.StopAsync(new() { Path = Path.Combine(directory, "trace.zip") });
            NUnit.Framework.TestContext.AddTestAttachment(Path.Combine(directory, "screenshot.png"));
            NUnit.Framework.TestContext.AddTestAttachment(Path.Combine(directory, "trace.zip"));
        }
        else
        {
            await _context.Tracing.StopAsync();
        }

        await _context.DisposeAsync();
    }

    private static async Task WaitForAppAsync(string baseUrl)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var deadline = DateTime.UtcNow.AddMinutes(2);
        while (true)
        {
            try
            {
                var response = await client.GetAsync(baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            if (DateTime.UtcNow > deadline)
            {
                throw new InvalidOperationException(
                    $"App is not reachable at {baseUrl}. Start it with 'docker compose up -d --build' in NRZMyk.BrowserTests.");
            }
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
