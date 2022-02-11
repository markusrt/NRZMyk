using System;
using System.Threading.Tasks;
using AutoMapper;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NRZMyk.Components.Pages.SentinelEntryPage;
using NRZMyk.Mocks.MockServices;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace NRZMyk.ComponentsTests.Pages.SentinelEntryPage
{
    public class CryoViewTests
    {
        private TestContext _context;
        private IRenderedComponent<CryoView> _renderedComponent;
        private SentinelEntryService _sentinelEntryService;

        [SetUp]
        public void CreateComponent()
        {
            MockSentinelEntryServiceImpl.Delay = 0;
            _context = new TestContext();
            _context.Services.AddAutoMapper(typeof(SentinelEntryService).Assembly);
            _context.Services.AddSingleton<SentinelEntryService, MockSentinelEntryServiceImpl>();
            _context.Services.AddSingleton<IAccountService, MockAccountService>();
            _context.Services.AddScoped(typeof(ILogger<>), typeof(NullLogger<>));
             
            _sentinelEntryService = _context.Services.GetService<SentinelEntryService>();
            _renderedComponent = CreateSut(_context);
        }

        [TearDown]
        public void DisposeContext()
        {
            _renderedComponent.Dispose();
            _context.Dispose();
        }

        [Test]
        public async Task WhenLoadDataWithEmptyResult_IndicatesMissingData()
        {
            var sut = _renderedComponent.Instance;

             sut.SelectedOrganization = 10;
            await sut.LoadData().ConfigureAwait(true);

            _renderedComponent.Markup.Should().Contain(
                "Zur gewählten Organisation wurden keine Einträge gefunden.");
        }

        [Test]
        public async Task WhenLoadDataWithSelectedOrganization_DisplaysEntries()
        {
            var sut = _renderedComponent.Instance;

            sut.SelectedOrganization = 1;
            await sut.LoadData().ConfigureAwait(true);

            _renderedComponent.Markup.Should().Contain("<td>SLN-123456</td>");
        }

        [Test]
        public async Task WhenPutToCryo_ShowsRemark()
        {
            var sut = _renderedComponent.Instance;

            sut.SelectedOrganization = 1;
            await sut.PutToCryoStorage(
                new SentinelEntry{Id=1, CryoRemark = "Duplicate to SN-133422"}).ConfigureAwait(true);

            _renderedComponent.Markup.Should().Contain("Duplicate to SN-133422");
            var entry = await _sentinelEntryService.GetById(1).ConfigureAwait(true);
            entry.CryoDate.Should().BeCloseTo(DateTime.Now);
        }
        
        [Test]
        public async Task WhenReleaseFromCryoStorage_ShowsRemarkAndClearsDate()
        {
            var sut = _renderedComponent.Instance;

            sut.SelectedOrganization = 1;
            await sut.ReleaseFromCryoStorage(
                new SentinelEntry{Id=1, CryoRemark = "Wrongly sent specimen"}).ConfigureAwait(true);

            _renderedComponent.Markup.Should().Contain("Wrongly sent specimen");
            var entry = await _sentinelEntryService.GetById(1).ConfigureAwait(true);
            entry.CryoDate.Should().BeNull();
        }

        private IRenderedComponent<CryoView> CreateSut(TestContext context)
        {
            return context.RenderComponent<CryoView>();
        }
    }
}