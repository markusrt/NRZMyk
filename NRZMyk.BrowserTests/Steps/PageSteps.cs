using Microsoft.Playwright;
using NRZMyk.BrowserTests.Support;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace NRZMyk.BrowserTests.Steps;

/// <summary>Steps that work on any page: navigation, buttons, visible texts and form fields.</summary>
[Binding]
public class PageSteps
{
    private readonly BrowserSession _browser;

    public PageSteps(BrowserSession browser)
    {
        _browser = browser;
    }

    private IPage Page => _browser.Page;

    [StepDefinition(@"I open the page {string}")]
    public async Task WhenIOpenThePage(string path)
    {
        await Page.GotoAsync(path);
    }

    [StepDefinition(@"I click {string}")]
    public async Task WhenIClick(string name)
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = name, Exact = true })
            .Or(Page.GetByRole(AriaRole.Link, new() { Name = name, Exact = true }))
            .First.ClickAsync();
    }

    [Then(@"I see the text {string}")]
    public async Task ThenISeeTheText(string text)
    {
        await Expect(Page.GetByText(text).First).ToBeVisibleAsync();
    }

    [Then(@"I see the texts:")]
    public async Task ThenISeeTheTexts(Table table)
    {
        foreach (var row in table.Rows)
        {
            await ThenISeeTheText(row[0]);
        }
    }

    [Then(@"I do not see the text {string}")]
    public async Task ThenIDoNotSeeTheText(string text)
    {
        await Expect(Page.GetByText(text)).ToHaveCountAsync(0);
    }

    [When(@"I enter the following values:")]
    public async Task WhenIEnterTheFollowingValues(Table table)
    {
        foreach (var row in table.Rows)
        {
            await FillAsync(Field(row["Field"]), row["Value"]);
        }
    }

    [Then(@"the field {string} has the value {string}")]
    public async Task ThenTheFieldHasTheValue(string label, string value)
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
