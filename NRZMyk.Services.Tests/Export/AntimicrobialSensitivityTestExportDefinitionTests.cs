using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using HaemophilusWeb.Tools;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NSubstitute;
using NUnit.Framework;
using Tynamix.ObjectFiller;

namespace NRZMyk.Services.Tests.Export
{
    public class AntimicrobialSensitivityTestExportDefinitionTests
    {
        private AntimicrobialSensitivityTest AntimicrobialSensitivityTest { get; set; }

        private List<AntimicrobialSensitivityTest> AntimicrobialSensitivityTests { get; set; }


        [SetUp]
        public void Setup()
        {
            var filler = new Filler<AntimicrobialSensitivityTest>();
            AntimicrobialSensitivityTest = filler.Create();
            
            var sentinelEntries = new List<AntimicrobialSensitivityTest> {AntimicrobialSensitivityTest};
            sentinelEntries.AddRange(filler.Create(10));
            AntimicrobialSensitivityTests = sentinelEntries;
        }


        [Test]
        public void Ctor_DoesNotThrow()
        {
            var sut = CreateExportDefinition(out _);

            sut.Should().NotBeNull();
        }

        [Test]
        public void DataTable_ContainsAllColumns()
        {
            var sut = CreateExportDefinition(out _);

            var export = sut.ToDataTable(AntimicrobialSensitivityTests);

            export.Columns.Count.Should().Be(6);
        }

        [Test]
        public void DataTable_ContainsValues()
        {
            var sut = CreateExportDefinition(out var micStepsService);

            AntimicrobialSensitivityTest.SentinelEntry.Id = 1234;
            AntimicrobialSensitivityTest.MinimumInhibitoryConcentration = 0.12f;
            AntimicrobialSensitivityTest.AntifungalAgent = AntifungalAgent.Itraconazole;
            AntimicrobialSensitivityTest.TestingMethod = SpeciesTestingMethod.Microdilution;
            AntimicrobialSensitivityTest.Resistance = Resistance.Resistant;
            AntimicrobialSensitivityTest.ClinicalBreakpoint.AntifungalAgentDetails = "Itraconazole";
            AntimicrobialSensitivityTest.ClinicalBreakpoint.Standard = BrothMicrodilutionStandard.Eucast;
            AntimicrobialSensitivityTest.ClinicalBreakpoint.Version = "10.0";
            AntimicrobialSensitivityTest.ClinicalBreakpoint.Species = Species.CandidaAlbicans;
            AntimicrobialSensitivityTest.ClinicalBreakpoint.ValidFrom = new DateTime(2020,02,04);

            var export = sut.ToDataTable(AntimicrobialSensitivityTests);

            export.Rows[0]["Sentinel Datensatz Id"].Should().Be(1234);
            export.Rows[0]["MHK"].Should().Be("0,12");
            export.Rows[0]["Antimykotikum"].Should().Be("Itraconazol");
            export.Rows[0]["Test"].Should().Be("Mikrodilution");
            export.Rows[0]["Bewertung"].Should().Be("resistent");
            export.Rows[0]["Breakpoint"].Should().Be("Itraconazole - Candida albicans - v10.0");
        }

        [Test]
        public void DataTable_ShowsGreaterThanAndSmallerThanAccordingly()
        {
            var sut = CreateExportDefinition(out var micStepsService);
            micStepsService.StepsByTestingMethodAndAgent(SpeciesTestingMethod.YeastOne, AntifungalAgent.Anidulafungin)
                .Returns(new List<MicStep>
                {
                    new MicStep {Title = ">8", Value = 8.001f}
                });

            AntimicrobialSensitivityTest.SentinelEntry.Id = 1234;
            AntimicrobialSensitivityTest.MinimumInhibitoryConcentration = 8.001f;
            AntimicrobialSensitivityTest.AntifungalAgent = AntifungalAgent.Anidulafungin;
            AntimicrobialSensitivityTest.TestingMethod = SpeciesTestingMethod.YeastOne;
            AntimicrobialSensitivityTest.Resistance = Resistance.Resistant;
            AntimicrobialSensitivityTest.ClinicalBreakpoint = null;

            var export = sut.ToDataTable(AntimicrobialSensitivityTests);

            export.Rows[0]["Sentinel Datensatz Id"].Should().Be(1234);
            export.Rows[0]["MHK"].Should().Be(">8");
        }

        [TestCase(SpeciesTestingMethod.ETest, "E-Test")]
        [TestCase(SpeciesTestingMethod.Microdilution, "Mikrodilution")]
        [TestCase(SpeciesTestingMethod.Micronaut, "Micronaut")]
        [TestCase(SpeciesTestingMethod.Vitek, "Vitek")]
        [TestCase(SpeciesTestingMethod.YeastOne, "Yeast One")]
        public void DataTable_ShowsTestingMethod(SpeciesTestingMethod testingMethod, string exportedTestingMethod)
        {
            var sut = CreateExportDefinition(out var micStepsService);
            micStepsService.StepsByTestingMethodAndAgent(SpeciesTestingMethod.YeastOne, AntifungalAgent.Anidulafungin)
                .Returns([new MicStep { Title = ">8", Value = 8.001f }]);

            AntimicrobialSensitivityTest.SentinelEntry.Id = 1234;
            AntimicrobialSensitivityTest.MinimumInhibitoryConcentration = 8.001f;
            AntimicrobialSensitivityTest.AntifungalAgent = AntifungalAgent.Anidulafungin;
            AntimicrobialSensitivityTest.TestingMethod = testingMethod;
            AntimicrobialSensitivityTest.Resistance = Resistance.Resistant;
            AntimicrobialSensitivityTest.ClinicalBreakpoint = null;

            var export = sut.ToDataTable(AntimicrobialSensitivityTests);

            export.Rows[0]["Test"].Should().Be(exportedTestingMethod);
        }

        private AntimicrobialSensitivityTestExportDefinition CreateExportDefinition(out IMicStepsService micStepsService)
        {
            micStepsService = Substitute.For<IMicStepsService>();
            micStepsService.StepsByTestingMethodAndAgent(Arg.Any<SpeciesTestingMethod>(), Arg.Any<AntifungalAgent>())
                .Returns(new List<MicStep>());
            return new AntimicrobialSensitivityTestExportDefinition(micStepsService);
        }
    }

}