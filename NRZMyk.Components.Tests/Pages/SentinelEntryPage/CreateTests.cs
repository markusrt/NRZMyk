using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AutoMapper;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NRZMyk.Components.Pages.SentinelEntryPage;
using NRZMyk.Mocks.MockServices;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NSubstitute;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace NRZMyk.ComponentsTests.Pages.SentinelEntryPage
{
    public class CreateTests
    {
        private TestContext _context;
        private MockClinicalBreakpointService _mockClinicalBreakpointService;
        private MockMicStepsService _mockMicStepsService;
        private ISentinelEntryService _sentinelEntryService;

        [SetUp]
        public void CreateComponent()
        {
            MockSentinelEntryServiceImpl.Delay = 0;
            _mockClinicalBreakpointService = new MockClinicalBreakpointService();
            _mockMicStepsService = new MockMicStepsService();

            _context = new TestContext();
            _context.Services.AddAutoMapper(typeof(ISentinelEntryService).Assembly);
            _context.Services.AddSingleton<ISentinelEntryService, MockSentinelEntryServiceImpl>();
            _context.Services.AddSingleton<IClinicalBreakpointService>(_mockClinicalBreakpointService);
            _context.Services.AddSingleton<IMicStepsService>(_mockMicStepsService);
            _context.Services.AddScoped<AuthenticationStateProvider, MockAuthStateProvider>();
            _context.Services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

            _sentinelEntryService = _context.Services.GetService<ISentinelEntryService>();
        }

        [TearDown]
        public void DisposeContext()
        {
            _context.Dispose();
        }

        [Test]
        public void WhenCreatedWithoutId_DoesInitializeData()
        {
            var component = CreateSut(builder => builder.Add(c => c.Id, null));
            var sut = component.Instance;

            component.Markup.Should().NotBeEmpty();
            sut.AllBreakpoints.Should().NotBeEmpty();
            sut.TestingMethod.Should().Be(SpeciesTestingMethod.Vitek);
            sut.AntifungalAgent.Should().Be(AntifungalAgent.Micafungin);
            sut.LaboratoryNumber.Should().BeEmpty();
            sut.CryoBox.Should().BeEmpty();
        }

        [Test]
        public void WhenCreatedWith_DoesInitializeData()
        {
            var component = CreateSut(builder => builder.Add(c => c.Id, 1));
            var sut = component.Instance;

            component.Markup.Should().NotBeEmpty();
            sut.AllBreakpoints.Should().NotBeEmpty();
            sut.TestingMethods().Should().NotBeEmpty();
            sut.LaboratoryNumber.Should().Be("SN-2020-0002");
            sut.CryoBox.Should().Be("SN-0000");
        }

        [Test]
        public async Task WhenSubmitWithId_UpdatesExistingEntryAndTriggersClose()
        {
            var closeTriggered = false;
            var component = CreateSut(builder =>
                    {
                        builder.Add(c => c.OnCloseClick, e =>
                        {
                            closeTriggered = true;
                        }).Add(c => c.Id, 1);
                    });
            var sut = component.Instance;
            sut.SentinelEntry.SenderLaboratoryNumber = "993882";

            await sut.SubmitClick();

            closeTriggered.Should().BeTrue();
            var entry = await _sentinelEntryService.GetById(1);
            entry.SenderLaboratoryNumber.Should().Be("993882");
        }

        [Test]
        public async Task WhenSubmitWithoutId_CreatesNewEntryAndTriggersClose()
        {
            var closeTriggered = false;
            var component = CreateSut(builder =>
            {
                builder.Add(c => c.OnCloseClick, e =>
                {
                    closeTriggered = true;
                });
            });
            var sut = component.Instance;
            sut.SentinelEntry.SenderLaboratoryNumber = "193882";

            await sut.SubmitClick();

            closeTriggered.Should().BeTrue();
            var entry = await _sentinelEntryService.GetById(3);
            entry.SenderLaboratoryNumber.Should().Be("193882");
        }

        [Test]
        public void WhenAddAntimicrobialSensitivityTestClicked_NewSensitivityTestIsAdded()
        {
            var component = CreateSut();
            var sut = component.Instance;

            var addTestButton = component.Find("#addAntimicrobialSensitivityTest");
            addTestButton.Click();

            sut.SentinelEntry.AntimicrobialSensitivityTests.Should().HaveCount(1);
            var sensitivityTest = sut.SentinelEntry.AntimicrobialSensitivityTests.First();

            sensitivityTest.TestingMethod.Should().Be(SpeciesTestingMethod.Vitek);
            sensitivityTest.AntifungalAgent.Should().Be(AntifungalAgent.AmphotericinB);
        }

        [Test]
        public void WhenAddAntimicrobialSensitivityTestClickedButNoBreakpoints_NewSensitivityTestIsAdded()
        {
            var component = CreateSut();
            var sut = component.Instance;
            var storedBreakpoints = new List<ClinicalBreakpoint>(sut.AllBreakpoints);
            try
            {
                sut.AllBreakpoints.Clear();

                var addTestButton = component.Find("#addAntimicrobialSensitivityTest");
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
            var component = CreateSut();
            var sut = component.Instance;
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaAlbicans;

            var breakpoints = sut.ApplicableBreakpoints(sensitivityTest).ToList();

            breakpoints.Should().HaveCount(3);
            breakpoints.Select(b => b.Version).Should().BeInDescendingOrder();
            breakpoints.Should().OnlyContain(
                b => b.AntifungalAgent == AntifungalAgent.Micafungin && b.Standard == BrothMicrodilutionStandard.Eucast);
        }

        [Test]
        public void WhenIdentifiedSpeciesIsChangedAfterAddingTests_BreakpointWithLatestVersionIsSelected()
        {
            var component = CreateSut();
            var sut = component.Instance;
            sut.TestingMethod = SpeciesTestingMethod.ETest;
            sut.AntifungalAgent = AntifungalAgent.Flucytosine;
            sut.AddAntimicrobialSensitivityTest();
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaAlbicans;

            var breakpoints = sut.ApplicableBreakpoints(sut.SentinelEntry.AntimicrobialSensitivityTests.Single()).ToList();

            var breakpointId = sut.SentinelEntry.AntimicrobialSensitivityTests.Single().ClinicalBreakpointId;
            breakpoints.Single(b=>b.Id == breakpointId).Version.Should().Be("12.0");
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
            var component = CreateSut();
            var sut = component.Instance;
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaTropicalis;

            sut.ApplicableBreakpoints(sensitivityTest);

            sensitivityTest.Resistance.Should().Be(Resistance.NotDetermined);
        }

        [Test]
        public void WhenNormalInternalUnitSelected_NormalTypeIsVisible()
        {
            var component = CreateSut();
            var sut = component.Instance;

            sut.SentinelEntry.HospitalDepartment = HospitalDepartment.Internal;
            sut.SentinelEntry.HospitalDepartmentType = HospitalDepartmentType.NormalUnit;

            sut.CheckInternalNormalTypeVisibility().Should().BeTrue();
        }

        [Test]
        public void WhenNormalInternalUnitDeSelected_NormalTypeIsCleared()
        {
            var component = CreateSut();
            var sut = component.Instance;
            sut.SentinelEntry.HospitalDepartment = HospitalDepartment.Internal;
            sut.SentinelEntry.HospitalDepartmentType = HospitalDepartmentType.NormalUnit;
            sut.SentinelEntry.InternalHospitalDepartmentType = InternalHospitalDepartmentType.Gastroenterological;

            sut.SentinelEntry.HospitalDepartment = HospitalDepartment.Dermatology;

            sut.CheckInternalNormalTypeVisibility().Should().BeFalse();
            sut.SentinelEntry.InternalHospitalDepartmentType.Should()
                .Be(InternalHospitalDepartmentType.NoInternalDepartment);
        }

        [TestCase(0.01f, "bg-danger", Resistance.Resistant)]
        [TestCase(0f, "bg-warning", Resistance.Intermediate)]
        [TestCase(-0.01f, "bg-warning", Resistance.Intermediate)]
        [TestCase(-0.2f, "bg-success", Resistance.Susceptible)]
        public void WhenCalculateResistanceBadge_UsesBreakpointValuesWithEucastBoundaryConditions(float deltaToResistance, string expectedBadge, Resistance expectedResistance)
        {
            var component = CreateSut();
            var sut = component.Instance;
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

        [Test]
        public void WhenCalculateResistanceBadge_FloorsMicToClosestReferenceValue()
        {
            var component = CreateSut();
            var sut = component.Instance;
            sut.SentinelEntry.IdentifiedSpecies = Species.CandidaKrusei;
            var breakpoint = sut.AllBreakpoints.Single(b => 
                b.AntifungalAgent == AntifungalAgent.Anidulafungin
                && b.Species == Species.CandidaKrusei
                && b.Standard == BrothMicrodilutionStandard.Eucast
                && b.Version == "10.0");
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = breakpoint.Id,
                MinimumInhibitoryConcentration = 0.064f
            };
            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistanceBadge(sensitivityTest);

            badge.Should().Be("bg-success");
            sensitivityTest.Resistance.Should().Be(Resistance.Susceptible);
        }

        [TestCase(0.01f, "bg-danger", Resistance.Resistant)]
        [TestCase(0f, "bg-danger", Resistance.Resistant)]
        [TestCase(-0.01f, "bg-warning", Resistance.Intermediate)]
        [TestCase(-1.0f, "bg-success", Resistance.Susceptible)]
        public void WhenCalculateResistanceBadge_UsesBreakpointValuesWithClsiBoundaryConditions(float deltaToResistance, string expectedBadge, Resistance expectedResistance)
        {
            var component = CreateSut();
            var sut = component.Instance;
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
        public void WhenCalculateResistanceBadgeWithLowerBoundaryStillBiggerThenSusceptible_ResultsInNotEvaluable()
        {
            var component = CreateSut();
            var sut = component.Instance;
            var breakPoint = new MockClinicalBreakpointService.MockClinicalBreakPoint()
            {
                AntifungalAgent = AntifungalAgent.Flucytosine,
                MicBreakpointSusceptible = 0.01f,
                MicBreakpointResistent = 100f,
                Species = Species.CandidaParapsilosis,
            };
            _mockClinicalBreakpointService.AddBreakpoint(100000, breakPoint);
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = 100000,
                AntifungalAgent = AntifungalAgent.Flucytosine,
                MinimumInhibitoryConcentration = _mockMicStepsService.StepsByTestingMethodAndAgent(SpeciesTestingMethod.Vitek, AntifungalAgent.Flucytosine).First().Value
            };

            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistanceBadge(sensitivityTest);

            badge.Should().Be("bg-info");
            sensitivityTest.Resistance.Should().Be(Resistance.NotEvaluable);
        }

        [Test]
        public void WhenCalculateResistanceBadgeWithUpperBoundaryStillSmallerThenResistant_ResultsInNotEvaluable()
        {
            var component = CreateSut();
            var sut = component.Instance;
            var breakPoint = new MockClinicalBreakpointService.MockClinicalBreakPoint()
            {
                AntifungalAgent = AntifungalAgent.Flucytosine,
                MicBreakpointSusceptible = 0.01f,
                MicBreakpointResistent = 100f,
                Species = Species.CandidaParapsilosis,
            };
            _mockClinicalBreakpointService.AddBreakpoint(100000, breakPoint);
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = 100000,
                AntifungalAgent = AntifungalAgent.Flucytosine,
                MinimumInhibitoryConcentration = _mockMicStepsService.StepsByTestingMethodAndAgent(SpeciesTestingMethod.Vitek, AntifungalAgent.Flucytosine).Last().Value
            };

            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistanceBadge(sensitivityTest);

            badge.Should().Be("bg-info");
            sensitivityTest.Resistance.Should().Be(Resistance.NotEvaluable);
        }

        [Test]
        public void WhenCalculateResistanceBadgeWithLowerBoundaryBiggerThenSusceptibleButSusceptibleIsUnrealisticSmall_ResultIsIntermediate()
        {
            var component = CreateSut();
            var sut = component.Instance;
            var breakPoint = new MockClinicalBreakpointService.MockClinicalBreakPoint()
            {
                AntifungalAgent = AntifungalAgent.Flucytosine,
                MicBreakpointSusceptible = 0.001f,
                MicBreakpointResistent = 100f,
                Species = Species.CandidaParapsilosis,
            };
            _mockClinicalBreakpointService.AddBreakpoint(100000, breakPoint);
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = 100000,
                AntifungalAgent = AntifungalAgent.Flucytosine,
                MinimumInhibitoryConcentration = _mockMicStepsService.StepsByTestingMethodAndAgent(SpeciesTestingMethod.Vitek, AntifungalAgent.Flucytosine).First().Value
            };

            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistanceBadge(sensitivityTest);

            badge.Should().Be("bg-warning");
            sensitivityTest.Resistance.Should().Be(Resistance.Intermediate);
        }

        [Test]
        public void WhenCalculateResistanceBadgeWithNoMatchingBreakpoint_ResultsInNotDetermined()
        {
            var component = CreateSut();
            var sut = component.Instance;
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

            badge.Should().Be("bg-info");
            sensitivityTest.Resistance.Should().Be(Resistance.NotDetermined);
        }

        [Test]
        public void WhenCalculateResistanceBadgeWhereBreakpointHasNoMics_ResultsInNotDetermined()
        {
            var component = CreateSut();
            var sut = component.Instance;
            var sensitivityTest = new AntimicrobialSensitivityTestRequest
            {
                ClinicalBreakpointId = 1234567,
                MinimumInhibitoryConcentration = 0.25f

            };
            sut.SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);

            var badge = sut.ResistanceBadge(sensitivityTest);

            badge.Should().Be("bg-info");
            sensitivityTest.Resistance.Should().Be(Resistance.NotDetermined);
        }

        [Test]
        public void WhenSensitivityTestsAreAdded_ListIsSortedBasedOnTestingMethodAndAntifungalAgentGroups()
        {
            var component = CreateSut();
            var sut = component.Instance;
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
        
        private IRenderedComponent<Create> CreateSut(Action<ComponentParameterCollectionBuilder<Create>> parameterBuilder = null)
        {
            return parameterBuilder == null
                ? _context.RenderComponent<Create>()
                : _context.RenderComponent(parameterBuilder);
        }
    }

    public class CreateServerValidationTests
    {
        private TestContext _context;
        private ISentinelEntryService _mockSentinelEntryService;

        [SetUp]
        public void CreateComponent()
        {
            _mockSentinelEntryService = Substitute.For<ISentinelEntryService>();
            _mockSentinelEntryService.Other(Arg.Any<string>())
                .Returns(Task.FromResult(new List<string> { "test1", "test2" }));

            _context = new TestContext();
            _context.Services.AddAutoMapper(typeof(ISentinelEntryService).Assembly);
            _context.Services.AddSingleton(_mockSentinelEntryService);
            _context.Services.AddSingleton<IClinicalBreakpointService>(new MockClinicalBreakpointService());
            _context.Services.AddSingleton<IMicStepsService>(new MockMicStepsService());
            _context.Services.AddScoped<AuthenticationStateProvider, MockAuthStateProvider>();
            _context.Services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        }

        [TearDown]
        public void DisposeContext()
        {
            _context.Dispose();
        }

        [Test]
        public async Task WhenSubmitThrowsServerValidationException_ValidationErrorIsAddedToEditContext()
        {
            var validationErrors = new Dictionary<string, string[]>
            {
                { "PredecessorLaboratoryNumber", new[] { "Die Labornummer wurde nicht gefunden" } }
            };
            _mockSentinelEntryService.Create(Arg.Any<SentinelEntryRequest>())
                .Returns<SentinelEntry>(_ => throw new ServerValidationException(validationErrors));

            var component = _context.RenderComponent<Create>();
            var sut = component.Instance;
            sut.SentinelEntry.SenderLaboratoryNumber = "test";

            await component.InvokeAsync(() => sut.SubmitClick());

            sut.EditContext.GetValidationMessages().Should().Contain("Die Labornummer wurde nicht gefunden");
            sut.SaveFailed.Should().BeFalse();
        }

        [Test]
        public async Task WhenSubmitThrowsServerValidationException_MultipleErrorsAreAddedToEditContext()
        {
            var validationErrors = new Dictionary<string, string[]>
            {
                { "PredecessorLaboratoryNumber", new[] { "Error 1", "Error 2" } },
                { "SenderLaboratoryNumber", new[] { "Error 3" } }
            };
            _mockSentinelEntryService.Create(Arg.Any<SentinelEntryRequest>())
                .Returns<SentinelEntry>(_ => throw new ServerValidationException(validationErrors));

            var component = _context.RenderComponent<Create>();
            var sut = component.Instance;

            await component.InvokeAsync(() => sut.SubmitClick());

            var messages = sut.EditContext.GetValidationMessages().ToList();
            messages.Should().HaveCount(3);
            messages.Should().Contain("Error 1");
            messages.Should().Contain("Error 2");
            messages.Should().Contain("Error 3");
        }

        [Test]
        public async Task WhenFieldIsEditedAfterServerValidationError_ErrorIsClearedForThatField()
        {
            var validationErrors = new Dictionary<string, string[]>
            {
                { "SenderLaboratoryNumber", new[] { "Server error message" } }
            };
            _mockSentinelEntryService.Create(Arg.Any<SentinelEntryRequest>())
                .Returns<SentinelEntry>(_ => throw new ServerValidationException(validationErrors));

            var component = _context.RenderComponent<Create>();
            var sut = component.Instance;

            await component.InvokeAsync(() => sut.SubmitClick());
            sut.EditContext.GetValidationMessages().Should().Contain("Server error message");

            // Simulate field change by modifying the value and notifying
            sut.SentinelEntry.SenderLaboratoryNumber = "new value";
            await component.InvokeAsync(() => 
                sut.EditContext.NotifyFieldChanged(new Microsoft.AspNetCore.Components.Forms.FieldIdentifier(sut.SentinelEntry, nameof(SentinelEntryRequest.SenderLaboratoryNumber))));

            sut.EditContext.GetValidationMessages().Should().NotContain("Server error message");
        }
    }
}