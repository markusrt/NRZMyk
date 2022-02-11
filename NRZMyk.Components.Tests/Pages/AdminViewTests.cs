using System;
using System.Threading.Tasks;
using AutoMapper;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NRZMyk.Components.Pages;
using NRZMyk.Mocks.MockServices;
using NRZMyk.Services.Services;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace NRZMyk.ComponentsTests.Pages.SentinelEntryPage
{
    public class AdminViewTests
    {
        private TestContext _context;
        private IAccountService _accountService;
        private IRenderedComponent<Admin> _renderedComponent;

        [SetUp]
        public void CreateComponent()
        {
            MockAccountService.Delay = 0;
            _context = new TestContext();
            _context.Services.AddSingleton<IAccountService, MockAccountService>();
            _context.Services.AddScoped(typeof(ILogger<>), typeof(NullLogger<>));
             
            _accountService = _context.Services.GetService<IAccountService>();
            _renderedComponent = CreateSut(_context);
        }

        [TearDown]
        public void DisposeContext()
        {
            _renderedComponent.Dispose();
            _context.Dispose();
        }

        [Test]
        public async Task WhenListAccounts_AddsAllTableEntries()
        {
            var accounts = await _accountService.ListAccounts();
            var sut = _renderedComponent.Instance;

            await sut.SubmitClick().ConfigureAwait(true);
            _renderedComponent.WaitForState(() => sut.SaveState == Components.Helpers.SaveState.Success);

            _renderedComponent.FindAll("tr").Should().HaveCount(accounts.Count+1);
            foreach (var account in accounts)
            {
               _renderedComponent.Markup.Should().Contain(account.DisplayName);
            }
        }
        private IRenderedComponent<Admin> CreateSut(TestContext context)
        {
            return context.RenderComponent<Admin>();
        }
    }
}