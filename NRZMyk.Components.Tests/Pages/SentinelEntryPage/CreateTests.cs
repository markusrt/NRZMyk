using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
            _renderedComponent.Instance?.SentinelEntry.AntimicrobialSensitivityTests.Clear();
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

            sut.SentinelEntry.AntimicrobialSensitivityTests.Should().HaveCount(1);
            var sensitivityTest = sut.SentinelEntry.AntimicrobialSensitivityTests.First();

            sensitivityTest.TestingMethod.Should().Be(SpeciesTestingMethod.Vitek);
            sensitivityTest.AntifungalAgent.Should().Be(AntifungalAgent.AmphotericinB);
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

                sut.SentinelEntry.AntimicrobialSensitivityTests.Should().HaveCount(1);
                var sensitivityTest = sut.SentinelEntry.AntimicrobialSensitivityTests.First();

                sensitivityTest.TestingMethod.Should().Be(SpeciesTestingMethod.Vitek);
                sensitivityTest.AntifungalAgent.Should().Be(AntifungalAgent.AmphotericinB);
                sensitivityTest.ClinicalBreakpointId.Should().BeNull();
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
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                AntifungalAgent = AntifungalAgent.Micafungin,
                Standard = BrothMicrodilutionStandard.Eucast
            };
            var sut = _renderedComponent.Instance;
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaAlbicans;

            var breakpoints = sut.ApplicableBreakpoints(sensitivityTest).ToList();

            breakpoints.Should().HaveCount(1);
            breakpoints.Should().OnlyContain(
                b => b.AntifungalAgent == AntifungalAgent.Micafungin && b.Standard == BrothMicrodilutionStandard.Eucast);
        }

        [Test]
        public void WhenNoApplicableBreakpoint_SensitivityIsSetToNotDetermined()
        {
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                AntifungalAgent = AntifungalAgent.Fluorouracil,
                Standard = BrothMicrodilutionStandard.Eucast,
                Resistance = Resistance.Susceptible
            };
            var sut = _renderedComponent.Instance;
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaTropicalis;

            sut.ApplicableBreakpoints(sensitivityTest);

            sensitivityTest.Resistance.Should().Be(Resistance.NotDetermined);
        }

        [TestCase(0.01f, "badge-danger", Resistance.Resistant)]
        [TestCase(0f, "badge-warning", Resistance.Intermediate)]
        [TestCase(-0.01f, "badge-warning", Resistance.Intermediate)]
        [TestCase(-0.2f, "badge-success", Resistance.Susceptible)]
        public void WhenCalculateResistanceBadge_UsesBreakpointValuesWithEucastBoundaryConditions(float deltaToResistance, string expectedBadge, Resistance expectedResistance)
        {
            var sut = _renderedComponent.Instance;
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaAlbicans;
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
            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistanceBadge(sensitivityTest);

            badge.Should().Be(expectedBadge);
            sensitivityTest.Resistance.Should().Be(expectedResistance);
        }

        [TestCase(0.01f, "badge-danger", Resistance.Resistant)]
        [TestCase(0f, "badge-danger", Resistance.Resistant)]
        [TestCase(-0.01f, "badge-warning", Resistance.Intermediate)]
        [TestCase(-1.0f, "badge-success", Resistance.Susceptible)]
        public void WhenCalculateResistanceBadge_UsesBreakpointValuesWithClsiBoundaryConditions(float deltaToResistance, string expectedBadge, Resistance expectedResistance)
        {
            var sut = _renderedComponent.Instance;
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaGlabrata;
            var breakpoint = sut.AllBreakpoints.Single(b => 
                b.AntifungalAgent == AntifungalAgent.Caspofungin
                && b.Species == Species.CandidaGlabrata
                && b.Standard == BrothMicrodilutionStandard.Clsi
                && b.Version == "M60 2019");
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = breakpoint.Id,
                MinimumInhibitoryConcentration = breakpoint.MicBreakpointResistent.Value + deltaToResistance

            };
            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistanceBadge(sensitivityTest);

            badge.Should().Be(expectedBadge);
            sensitivityTest.Resistance.Should().Be(expectedResistance);
        }

        [Test]
        public void WhenCalculateResistanceBadgeWithNoMatchingBreakpoint_ResultsInNotDetermined()
        {
            var sut = _renderedComponent.Instance;
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaAlbicans;
            var firstBreakpoint = sut.AllBreakpoints.First(b => 
                !b.MicBreakpointResistent.HasValue && !b.MicBreakpointSusceptible.HasValue);
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = firstBreakpoint.Id,
                MinimumInhibitoryConcentration = 0.25f
            };
            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistanceBadge(sensitivityTest);

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
            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistanceBadge(sensitivityTest);

            badge.Should().Be("badge-info");
            sensitivityTest.Resistance.Should().Be(Resistance.NotDetermined);
        }

        [Test]
        public void WhenSensitivityTestsAreAdded_ListIsSortedBasedOnTestingMethodAndAntifungalAgentGroups()
        {
            var sut = _renderedComponent.Instance;
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaAlbicans;
            var firstBreakpoint = sut.AllBreakpoints.First(b => 
                !b.MicBreakpointResistent.HasValue && !b.MicBreakpointSusceptible.HasValue);

            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = firstBreakpoint.Id,
                TestingMethod = SpeciesTestingMethod.ETest,
                AntifungalAgent = AntifungalAgent.Fluorouracil
            });
            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = firstBreakpoint.Id,
                TestingMethod = SpeciesTestingMethod.ETest,
                AntifungalAgent = AntifungalAgent.AmphotericinB
            });
            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = firstBreakpoint.Id,
                TestingMethod = SpeciesTestingMethod.Micronaut,
                AntifungalAgent = AntifungalAgent.Fluconazole
            });

            var viewOrder = sut.RecalculateResistance().ToList();

            viewOrder[0].TestingMethod.Should().Be(SpeciesTestingMethod.Micronaut);
            viewOrder[1].TestingMethod.Should().Be(SpeciesTestingMethod.ETest);
            viewOrder[1].AntifungalAgent.Should().Be(AntifungalAgent.AmphotericinB);
            viewOrder[2].TestingMethod.Should().Be(SpeciesTestingMethod.ETest);
            viewOrder[2].AntifungalAgent.Should().Be(AntifungalAgent.Fluorouracil);
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