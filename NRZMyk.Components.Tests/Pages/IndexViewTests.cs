using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NRZMyk.Components.Pages;
using NRZMyk.Mocks.MockServices;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using TestContext = Bunit.TestContext;

namespace NRZMyk.Components.Tests.Pages
{
    public class IndexViewTests
    {
        private TestContext _context;
        private IAccountService _accountService;
        private IReminderService _reminderService;
        private IRenderedComponent<Index> _renderedComponent;

        [SetUp]
        public void CreateComponent()
        {
            MockAccountService.Delay = 0;
            _context = new TestContext();

            _reminderService = Substitute.For<IReminderService>();

            _context.Services.AddAutoMapper(typeof(ISentinelEntryService).Assembly);
            _context.Services.AddSingleton<IAccountService, MockAccountService>();
            _context.Services.AddScoped(typeof(ILogger<>), typeof(NullLogger<>));
            _context.Services.AddScoped(typeof(IReminderService), _ => _reminderService);
            
            var authContext = _context.AddTestAuthorization();
            authContext.SetAuthorized("Mrs. Mock");
            authContext.SetRoles(Role.User.ToString());

            _accountService = _context.Services.GetService<IAccountService>();
            _reminderService = _context.Services.GetService<IReminderService>();
            _reminderService.HumanReadableExpectedNextSending(Arg.Any<Organization>()).Returns("Fake");
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
            var organizations = await _accountService.ListOrganizations();

            _renderedComponent.FindAll("tr").Should().HaveCount(organizations.Count+1);
        }

        private IRenderedComponent<Index> CreateSut(TestContext context)
        {
            return context.RenderComponent<Index>();
        }
    }
}