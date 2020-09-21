using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NRZMyk.Components.Pages.SentinelEntryPage;
using NRZMyk.Mocks.MockServices;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace NRZMyk.ComponentsTests.Pages.SentinelEntryPage
{
    public class CreateTests
    {
        private TestContext _context;
        private IRenderedComponent<Create> _renderedComponent;

        [OneTimeSetUp]
        public void CreateComponent()
        {
            _context = CreateContext();
            _renderedComponent = CreateSut(_context);
        }

        [OneTimeTearDown]
        public void DisposeContext()
        {
            _context.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _renderedComponent.Instance?.NewSentinelEntry.AntimicrobialSensitivityTests.Clear();
        }

        [Test]
        public void WhenCreated_DoesInitializeData()
        {
            var sut = _renderedComponent.Instance;

            _renderedComponent.Markup.Should().NotBeEmpty();
            sut.AllBreakpoints.Should().NotBeEmpty();
            sut.TestingMethod.Should().Be(SpeciesTestingMethod.Vitek);
            sut.AntifungalAgent.Should().Be(AntifungalAgent.Micafungin);
        }

        [Test]
        public void WhenAddAntimicrobialSensitivityTestClicked_NewSensitivityTestIsAdded()
        {
            var sut = _renderedComponent.Instance;

            var addTestButton = _renderedComponent.Find("#addAntimicrobialSensitivityTest");
            addTestButton.Click();

            sut.NewSentinelEntry.AntimicrobialSensitivityTests.Should().HaveCount(1);
            var sensitivityTest = sut.NewSentinelEntry.AntimicrobialSensitivityTests.First();

            sensitivityTest.TestingMethod.Should().Be(SpeciesTestingMethod.Vitek);
            sensitivityTest.AntifungalAgent.Should().Be(AntifungalAgent.Micafungin);
        }

        [Test]
        public void WhenAddAntimicrobialSensitivityTestClickedButNoBreakpoints_NewSensitivityTestIsAdded()
        {
            var sut = _renderedComponent.Instance;
            var storedBreakpoints = new List<ClinicalBreakpoint>(sut.AllBreakpoints);
            try
            {
                sut.AllBreakpoints.Clear();

                var addTestButton = _renderedComponent.Find("#addAntimicrobialSensitivityTest");
                addTestButton.Click();

                sut.NewSentinelEntry.AntimicrobialSensitivityTests.Should().HaveCount(1);
                var sensitivityTest = sut.NewSentinelEntry.AntimicrobialSensitivityTests.First();

                sensitivityTest.TestingMethod.Should().Be(SpeciesTestingMethod.Vitek);
                sensitivityTest.AntifungalAgent.Should().Be(AntifungalAgent.Micafungin);
                sensitivityTest.ClinicalBreakpointId.Should().Be(0);
            }
            catch (Exception e)
            {
                Assert.Fail($"Test failed due to exception {e}");
            }
            finally
            {
                //TODO find a way to create a new SUT for each test
                //(this failed previously with threading exception in component)
                sut.AllBreakpoints.AddRange(storedBreakpoints);
            }
        }

        [Test]
        public void WhenApplicableBreakpoints_OnlyValidOnesAreReturned()
        {
            var sut = _renderedComponent.Instance;
            sut.NewSentinelEntry.IdentifiedSpecies = Species.CandidaAlbicans;

            var breakpoints = sut.ApplicableBreakpoints(AntifungalAgent.Micafungin).ToList();

            breakpoints.Should().HaveCount(2);
            breakpoints.Should().OnlyContain(b => b.AntifungalAgent == AntifungalAgent.Micafungin);
        }

        [TestCase(0.01f, "badge-danger", Resistance.Resistant)]
        [TestCase(-0.01f, "badge-warning", Resistance.Intermediate)]
        [TestCase(-0.2f, "badge-success", Resistance.Susceptible)]
        public void WhenCalculateResistanceBadge_UsesBreakpointValues(float deltaToResistance, string expectedBadge, Resistance expectedResistance)
        {
            var sut = _renderedComponent.Instance;
            sut.NewSentinelEntry.IdentifiedSpecies = Species.CandidaAlbicans;
            var firstBreakpoint = sut.AllBreakpoints.Single(b => 
                    b.AntifungalAgent == AntifungalAgent.Voriconazole
                    && b.Species == Species.CandidaDubliniensis
                    && b.Standard == BrothMicrodilutionStandard.Eucast
                    && b.Version == "10.0");
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = firstBreakpoint.Id,
                MinimumInhibitoryConcentration = firstBreakpoint.MicBreakpointResistent.Value + deltaToResistance

            };
            sut.NewSentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistenceBadge(sensitivityTest);

            badge.Should().Be(expectedBadge);
            sensitivityTest.Resistance.Should().Be(expectedResistance);
        }

        [Test]
        public void WhenCalculateResistanceBadgeWithNoMatchingBreakpoint_ResultsInNotDetermined()
        {
            var sut = _renderedComponent.Instance;
            sut.NewSentinelEntry.IdentifiedSpecies = Species.CandidaAlbicans;
            var firstBreakpoint = sut.AllBreakpoints.First(b => 
                !b.MicBreakpointResistent.HasValue && !b.MicBreakpointSusceptible.HasValue);
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = firstBreakpoint.Id,
                MinimumInhibitoryConcentration = 0.25f
            };
            sut.NewSentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistenceBadge(sensitivityTest);

            badge.Should().Be("badge-info");
            sensitivityTest.Resistance.Should().Be(Resistance.NotDetermined);
        }

        [Test]
        public void WhenCalculateResistanceBadgeWhereBreakpointHasNoMics_ResultsInNotDetermined()
        {
            var sut = _renderedComponent.Instance;
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = 1234567,
                MinimumInhibitoryConcentration = 0.25f

            };
            sut.NewSentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistenceBadge(sensitivityTest);

            badge.Should().Be("badge-info");
            sensitivityTest.Resistance.Should().Be(Resistance.NotDetermined);
        }

        private static TestContext CreateContext()
        {
            var context = new TestContext();
            context.Services.AddAutoMapper(typeof(SentinelEntryService).Assembly);
            context.Services.AddSingleton<SentinelEntryService, MockSentinelEntryServiceImpl>();
            context.Services.AddSingleton<ClinicalBreakpointService, MockClinicalBreakpointServiceImpl>();
            context.Services.AddSingleton<MicStepsService, MockMicStepsService>();
            context.Services.AddScoped<AuthenticationStateProvider, MockAuthStateProvider>();
            context.Services.AddScoped<SignOutSessionStateManager>();
            context.Services.AddScoped(typeof(ILogger<>), typeof(NullLogger<>));
            return context;
        }

        private IRenderedComponent<Create> CreateSut(TestContext context)
        {
            return context.RenderComponent<Create>();
        }
    }
}