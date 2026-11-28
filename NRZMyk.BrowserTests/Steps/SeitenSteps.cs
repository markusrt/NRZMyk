using Microsoft.Playwright;
using NRZMyk.BrowserTests.Support;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace NRZMyk.BrowserTests.Steps;

/// <summary>Steps that work on any page: navigation, buttons, visible texts and form fields.</summary>
[Binding]
public class SeitenSteps
{
    private readonly BrowserSession _browser;

    public SeitenSteps(BrowserSession browser)
    {
        _browser = browser;
    }

    private IPage Page => _browser.Page;

    [StepDefinition(@"ich die Seite {string} öffne")]
    public async Task WennIchDieSeiteOeffne(string path)
    {
        await Page.GotoAsync(path);
    }

    [StepDefinition(@"ich auf {string} klicke")]
    public async Task WennIchAufKlicke(string name)
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = name, Exact = true })
            .Or(Page.GetByRole(AriaRole.Link, new() { Name = name, Exact = true }))
            .First.ClickAsync();
    }

    [Then(@"sehe ich den Text {string}")]
    public async Task DannSeheIchDenText(string text)
    {
        await Expect(Page.GetByText(text).First).ToBeVisibleAsync();
    }

    [Then(@"sehe ich die Texte:")]
    public async Task DannSeheIchDieTexte(Table table)
    {
        foreach (var row in table.Rows)
        {
            await DannSeheIchDenText(row[0]);
        }
    }

    [Then(@"sehe ich nicht den Text {string}")]
    public async Task DannSeheIchNichtDenText(string text)
    {
        await Expect(Page.GetByText(text)).ToHaveCountAsync(0);
    }

    [When(@"ich folgende Werte eingebe:")]
    public async Task WennIchFolgendeWerteEingebe(Table table)
    {
        foreach (var row in table.Rows)
        {
            await FillAsync(Field(row["Feld"]), row["Wert"]);
        }
    }

    [Then(@"hat das Feld {string} den Wert {string}")]
    public async Task DannHatDasFeldDenWert(string label, string value)
    {
        await Expect(Field(label)).ToHaveValueAsync(value);
    }

    private static async Task FillAsync(ILocator field, string value)
    {
        if (await field.EvaluateAsync<string>("e => e.tagName") == "SELECT")
        {
            await field.SelectOptionAsync(new SelectOptionValue { Label = value });
        }
        else
        {
            await field.FillAsync(value);
            // Blazor's @bind-Value on inputs reacts to "change", which fill alone does not raise
            await field.DispatchEventAsync("change");
        }
    }

    /// <summary>
    /// The app's forms place a label next to the field without a "for" attribute,
    /// so the field is the first input in the element after the label.
    /// </summary>
    private ILocator Field(string label) =>
        Page.Locator($"xpath=//label[normalize-space()='{label}']/following-sibling::div[1]")
            .Locator("select, input, textarea").First;
}
