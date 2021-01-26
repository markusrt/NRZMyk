﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NUnit.Framework;
using NRZMyk.Services.Services;
using NRZMyk.Services.Utils;

namespace NRZMyk.Services.Tests.Services
{
    public class MicStepServiceImplTests
    {
        [Test]
        public void WhenNotConfigured_ReturnsEmptyList()
        {
            var sut = CreateSut(Options.Create(new BreakpointSettings()));

            var steps = sut.StepsByTestingMethodAndAgent(SpeciesTestingMethod.ETest, AntifungalAgent.Caspofungin);
            
            steps.Should().BeEmpty();
        }

        [Test]
        public void WhenStandardsNotConfigured_ReturnsAllValues()
        {
            var expectedStandards = EnumUtils.AllEnumValues<BrothMicrodilutionStandard>().ToList();
            expectedStandards.Remove(BrothMicrodilutionStandard.None);
            var sut = CreateSut(Options.Create(new BreakpointSettings()));

            var standards = sut.Standards(SpeciesTestingMethod.ETest);

            standards.Should().BeEquivalentTo(expectedStandards);
        }

        [Test]
        public void WhenConfiguredWithoutSteps_ReturnsEmptyList()
        {
            var sut = CreateSut(Options.Create(new BreakpointSettings {Breakpoint = new Breakpoint()}));

            var steps = sut.StepsByTestingMethodAndAgent(SpeciesTestingMethod.ETest, AntifungalAgent.Caspofungin);
            
            steps.Should().BeEmpty();
        }

        [Test]
        public void WhenStandardConfigured_ReturnsCorrespondingValues()
        {
            var sut = CreateSut(Options.Create(new BreakpointSettings
            {Breakpoint = new Breakpoint
            {Standards = new Dictionary<SpeciesTestingMethod, List<BrothMicrodilutionStandard>> {{
                SpeciesTestingMethod.Vitek, new List<BrothMicrodilutionStandard> {BrothMicrodilutionStandard.Eucast}
            }}}}));

            var standards = sut.Standards(SpeciesTestingMethod.Vitek);

            standards.Should().HaveCount(1);
            standards.Should().OnlyContain(standard => standard ==BrothMicrodilutionStandard.Eucast);
        }

        [Test]
        public void WhenConfiguredWithEmptySteps_ReturnsEmptyList()
        {
            var sut = CreateSut(Options.Create(new BreakpointSettings
            {
                Breakpoint = new Breakpoint
                {
                    MicSteps = new Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>>()
                }
            }));

            var steps = sut.StepsByTestingMethodAndAgent(SpeciesTestingMethod.ETest, AntifungalAgent.Caspofungin);
            
            steps.Should().BeEmpty();
        }

        [Test]
        public void WhenConfiguredWithSteps_ReturnsMatchingList()
        {
            var expectedSteps = new List<MicStep>
            {
                new MicStep {Title = "<10", Value = 10},
                new MicStep {Title = "20", Value = 20}
            };
            var sut = CreateSutWithSteps();

            var steps = sut.StepsByTestingMethodAndAgent(SpeciesTestingMethod.ETest, AntifungalAgent.Caspofungin);
            
            steps.Should().HaveCount(2);
            steps.Should().BeEquivalentTo(expectedSteps);
        }

        [Test]
        public void WhenConfiguredWithSteps_ReturnsAllTestingMethods()
        {
            var expectedTestingMethods = new List<SpeciesTestingMethod>
            {
                SpeciesTestingMethod.ETest, SpeciesTestingMethod.Vitek
            };
            var sut = CreateSutWithSteps();

            var testingMethods = sut.TestingMethods();
            
            testingMethods.Should().HaveCount(2);
            testingMethods.Should().BeEquivalentTo(expectedTestingMethods);
        }

        [Test]
        public void WhenConfiguredWithSteps_ReturnsAllAntifungalAgents()
        {
            var sut = CreateSutWithSteps();

            var antifungalAgents = sut.AntifungalAgents(SpeciesTestingMethod.Vitek);
            
            antifungalAgents.Should().HaveCount(1);
            antifungalAgents.Should().BeEquivalentTo(new List<AntifungalAgent>{AntifungalAgent.Fluconazole});
        }

        [Test]
        public void WhenConfiguredWithSteps_ReturnsAntifungalAgentsInPreferredOrder()
        {
            var sut = CreateSutWithSteps();

            var antifungalAgents = sut.AntifungalAgents(SpeciesTestingMethod.ETest);
            
            antifungalAgents.Should().HaveCount(2);
            antifungalAgents.First().Should().Be(AntifungalAgent.Caspofungin);
        }

        private MicStepsServiceImpl CreateSutWithSteps()
        {
            var sut = CreateSut(Options.Create(new BreakpointSettings
            {
                Breakpoint = new Breakpoint
                {
                    MicSteps = new Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>>
                    {
                        [SpeciesTestingMethod.ETest] = new Dictionary<AntifungalAgent, List<MicStep>>
                        {
                            [AntifungalAgent.Micafungin] = new List<MicStep>
                            {
                                new MicStep {Title = "<11", Value = 11},
                            },
                            [AntifungalAgent.Caspofungin] = new List<MicStep>
                            {
                                new MicStep {Title = "<10", Value = 10},
                                new MicStep {Title = "20", Value = 20}
                            }
                        },
                        [SpeciesTestingMethod.Vitek] = new Dictionary<AntifungalAgent, List<MicStep>>
                        {
                            [AntifungalAgent.Fluconazole] = new List<MicStep>
                            {
                                new MicStep {Title = "<30", Value = 30},
                                new MicStep {Title = "40", Value = 40},
                                new MicStep {Title = "50", Value = 50}
                            }
                        }
                    }
                }
            }));
            return sut;
        }

        private MicStepsServiceImpl CreateSut(IOptions<BreakpointSettings> option)
        {
            return new MicStepsServiceImpl(option, new NullLogger<MicStepsServiceImpl>());
        }
    }
}