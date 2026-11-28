using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;

namespace NRZMyk.BrowserTests.Support;

[Binding]
public sealed class Hooks
{
    private static readonly string ResultsDirectory = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "TestResults");

    private static IPlaywright? _playwright;
    private static IBrowser? _browser;

    private readonly IObjectContainer _container;
    private readonly ScenarioContext _scenario;
    private BrowserSession? _session;

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
        await TestDatabase.ResetAsync(settings);

        _session = new BrowserSession(_browser!, settings);
        await _session.OpenAsync();
        _container.RegisterInstanceAs(_session);
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_session == null)
        {
            return;
        }

        string? failureDirectory = null;
        if (_scenario.TestError != null)
        {
            var name = Regex.Replace(_scenario.ScenarioInfo.Title, @"[^\w\-]+", "_");
            failureDirectory = Path.GetFullPath(Path.Combine(ResultsDirectory, $"{DateTime.Now:yyyyMMdd-HHmmss}_{name}"));
        }
        await _session.CloseAsync(failureDirectory);
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
