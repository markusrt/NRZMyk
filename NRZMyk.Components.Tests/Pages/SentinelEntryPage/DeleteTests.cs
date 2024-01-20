using System.Threading.Tasks;
using AutoMapper;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NRZMyk.Components.Pages.SentinelEntryPage;
using NRZMyk.Mocks.MockServices;
using NRZMyk.Services.Services;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace NRZMyk.ComponentsTests.Pages.SentinelEntryPage
{
    public class DeleteTests
    {
        private TestContext _context;
        private ISentinelEntryService _sentinelEntryService;
        private IRenderedComponent<Delete> _renderedComponent;

        [SetUp]
        public void CreateComponent()
        {
            MockSentinelEntryServiceImpl.Delay = 0;
            _context = new TestContext();
            _context.Services.AddAutoMapper(typeof(ISentinelEntryService).Assembly);
            _context.Services.AddSingleton<ISentinelEntryService, MockSentinelEntryServiceImpl>();
            _context.Services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
             
            _sentinelEntryService = _context.Services.GetService<ISentinelEntryService>();
            _renderedComponent = CreateSut(_context);
        }

        [TearDown]
        public void DisposeContext()
        {
            _renderedComponent.Dispose();
            _context.Dispose();
        }

        [Test]
        public async Task WhenDeleteClick_DeletesSelectedEntry()
        {
            var sut = _renderedComponent.Instance;

            sut.Id = 1;
            await sut.DeleteClick().ConfigureAwait(true);

            var deletedEntry = await _sentinelEntryService.GetById(1);
            deletedEntry.Should().BeNull();
        }
        private static IRenderedComponent<Delete> CreateSut(TestContext context)
        {
            return context.RenderComponent<Delete>();
        }
    }
}