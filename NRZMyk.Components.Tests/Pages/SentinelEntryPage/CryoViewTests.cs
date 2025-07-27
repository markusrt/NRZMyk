using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using NRZMyk.Components.Pages.SentinelEntryPage;
using NRZMyk.Mocks.MockServices;
using NRZMyk.Services.Services;
using NSubstitute;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace NRZMyk.Components.Tests.Pages.SentinelEntryPage
{
    public class CryoViewTests
    {
        private TestContext _context;
        private IRenderedComponent<CryoView> _renderedComponent;
        private MockSentinelEntryServiceImpl _sentinelEntryService;

        [SetUp]
        public void CreateComponent()
        {
            MockSentinelEntryServiceImpl.Delay = 0;
            _context = new TestContext();
            _context.Services.AddAutoMapper(typeof(ISentinelEntryService).Assembly);
            _context.Services.AddSingleton<ISentinelEntryService, MockSentinelEntryServiceImpl>();
            _context.Services.AddSingleton<IAccountService, MockAccountService>();
            _context.Services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
             
            _sentinelEntryService = _context.Services.GetService<ISentinelEntryService>() as MockSentinelEntryServiceImpl;
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
            await sut.LoadData().ConfigureAwait(true);

            var cryoEntry = sut.SentinelEntries.First();
            cryoEntry.CryoRemark = "Duplicate to SN-133422";
            await sut.PutToCryoStorage(cryoEntry).ConfigureAwait(true);

            _renderedComponent.Markup.Should().Contain("Duplicate to SN-133422");
            var entry = await _sentinelEntryService.GetById(1).ConfigureAwait(true);
            entry.CryoDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
        }
        
        [Test]
        public async Task WhenReleaseFromCryoStorage_ShowsRemarkAndClearsDate()
        {
            var sut = _renderedComponent.Instance;
            sut.SelectedOrganization = 1;
            await sut.LoadData().ConfigureAwait(true);

            var cryoEntry = sut.SentinelEntries.First();
            await sut.ReleaseFromCryoStorage(cryoEntry).ConfigureAwait(true);

            var entry = await _sentinelEntryService.GetById(1).ConfigureAwait(true);
            entry.CryoDate.Should().BeNull();
        }

        [Test]
        public async Task WhenEditClick_ShowsDetails()
        {
            var sut = _renderedComponent.Instance;
            sut.SelectedOrganization = 1;
            await sut.LoadData().ConfigureAwait(true);

            sut.EditClick(1);

            sut.SelectedId.Should().Be(1);
            sut.ShowEdit.Should().BeTrue();
        }

        [Test]
        public async Task WhenCloseEditHandler_UpdatesSelectedEntry()
        {
            var jsRuntime = Substitute.For<IJSRuntime>();
            var sut = _renderedComponent.Instance;
            sut.JsRuntime = jsRuntime;
            sut.SelectedOrganization = 1;
            await sut.LoadData().ConfigureAwait(true);
            var allEntries = await _sentinelEntryService.ListPaged(100).ConfigureAwait(true);
            var updatedEntry = allEntries.Single(s => s.Id == 1);
            updatedEntry.Remark = "Updated by test";

            sut.EditClick(1);
            await sut.CloseEditHandler("");

            await jsRuntime.Received().InvokeAsync<object>("closeBootstrapModal",
                Arg.Is<object[]>(o => o.Contains("editModal")));
            sut.SelectedId.Should().Be(0);
            sut.ShowEdit.Should().BeFalse();
            sut.SentinelEntries.Single(s => s.Id == 1).Remark.Should().Be("Updated by test");
        }

        [Test]
        public async Task WhenUpdateCryoRemark_UpdatesRemarkOnly()
        {
            var sut = _renderedComponent.Instance;
            sut.SelectedOrganization = 1;
            await sut.LoadData().ConfigureAwait(true);

            var cryoEntry = sut.SentinelEntries.First();
            var originalCryoDate = cryoEntry.CryoDate;
            cryoEntry.CryoRemark = "Updated cryo remark";
            await sut.UpdateCryoRemark(cryoEntry).ConfigureAwait(true);

            var entry = await _sentinelEntryService.GetById(1).ConfigureAwait(true);
            entry.CryoRemark.Should().Be("Updated cryo remark");
            entry.CryoDate.Should().Be(originalCryoDate); // CryoDate should remain unchanged
        }

        [Test]
        public async Task WhenEntriesInCryoStorage_ShowsSaveButton()
        {
            var sut = _renderedComponent.Instance;
            sut.SelectedOrganization = 1;
            await sut.LoadData().ConfigureAwait(true);

            // First entry has a CryoDate, so it should show the save button
            _renderedComponent.Markup.Should().Contain("title=\"Zusätzliche Info speichern\"");
            _renderedComponent.Markup.Should().Contain("oi-check");
        }

        [Test]
        public async Task WhenEntriesNotInCryoStorage_DoesNotShowSaveButton()
        {
            var sut = _renderedComponent.Instance;
            sut.SelectedOrganization = 2; // Second entry has no CryoDate
            await sut.LoadData().ConfigureAwait(true);

            // Should not show save button for non-cryo-stored entries
            _renderedComponent.Markup.Should().NotContain("title=\"Kryo-Bemerkung speichern\"");
            _renderedComponent.Markup.Should().Contain("title=\"In Cryobox einlagern\"");
        }

        [Test]
        public async Task WhenCryoRemarkInputTriggered_EnablesSaveButton()
        {
            var sut = _renderedComponent.Instance;
            sut.SelectedOrganization = 1;
            await sut.LoadData().ConfigureAwait(true);

            var cryoEntry = sut.SentinelEntries.First();
            
            // Initially save button should be disabled
            sut.HasCryoRemarkChanged(cryoEntry).Should().BeFalse();
            
            // Trigger the input callback
            sut.OnCryoRemarkInput(cryoEntry);
            
            // Now save button should be enabled
            sut.HasCryoRemarkChanged(cryoEntry).Should().BeTrue();
        }

        [Test]
        public async Task WhenCryoRemarkSaved_DisablesSaveButton()
        {
            var sut = _renderedComponent.Instance;
            sut.SelectedOrganization = 1;
            await sut.LoadData().ConfigureAwait(true);

            var cryoEntry = sut.SentinelEntries.First();
            
            // Trigger input to enable save button
            sut.OnCryoRemarkInput(cryoEntry);
            sut.HasCryoRemarkChanged(cryoEntry).Should().BeTrue();
            
            // Save the remark
            cryoEntry.CryoRemark = "Updated remark";
            await sut.UpdateCryoRemark(cryoEntry).ConfigureAwait(true);
            
            // Save button should be disabled again
            sut.HasCryoRemarkChanged(cryoEntry).Should().BeFalse();
        }

        private static IRenderedComponent<CryoView> CreateSut(TestContext context)
        {
            return context.RenderComponent<CryoView>();
        }
    }
}