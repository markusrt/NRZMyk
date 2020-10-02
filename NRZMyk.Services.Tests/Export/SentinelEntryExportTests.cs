using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HaemophilusWeb.Tools;
using NRZMyk.Services.Data.Entities;
using NUnit.Framework;
using Tynamix.ObjectFiller;

namespace NRZMyk.Services.Tests.Export
{
    public class SentinelEntryExportTests
    {
        private IEnumerable<SentinelEntry> SentinelEntries { get; set; }
        
        private SentinelEntry SentinelEntry { get; set; }

        [SetUp]
        public void Setup()
        {
            var filler = new Filler<SentinelEntry>();
            SentinelEntry = filler.Create();
            
            var sentinelEntries = new List<SentinelEntry> {SentinelEntry};
            sentinelEntries.AddRange(filler.Create(10));
            SentinelEntries = sentinelEntries;
        }


        [Test]
        public void Ctor_DoesNotCrash()
        {
            var sut = CreateExportDefinition();

            sut.Should().NotBeNull();
        }

        [Test]
        public void DataTable_ContainsAllColumns()
        {
            var sut = CreateExportDefinition();

            var export = sut.ToDataTable(SentinelEntries);

            export.Columns.Count.Should().Be(11);
        }

        [Test]
        public void DataTable_ContainsValues()
        {
            var sut = CreateExportDefinition();

            SentinelEntry.AgeGroup = AgeGroup.EightySixToNinety;
            SentinelEntry.SamplingDate = new DateTime(2005, 8, 31);
            SentinelEntry.HospitalDepartmentType = HospitalDepartmentType.NormalUnit;
            SentinelEntry.HospitalDepartment = HospitalDepartment.Anaesthetics;
            SentinelEntry.IdentifiedSpecies = Species.CandidaDubliniensis;
            SentinelEntry.Material = Material.Isolate;
            SentinelEntry.SenderLaboratoryNumber = "LabNr. 123";
            SentinelEntry.Year = 2020;
            SentinelEntry.YearlySequentialEntryNumber = 123;
            SentinelEntry.CryoBoxNumber = 5;
            SentinelEntry.CryoBoxSlot = 67;

            var export = sut.ToDataTable(SentinelEntries);

            export.Rows[0]["Labornummer"].ToString().Should().Match("SN-2020-0123");
            export.Rows[0]["Kryo-Box"].ToString().Should().Match("2020-005");
            export.Rows[0]["Altersgruppe"].Should().Be("86-90");
            export.Rows[0]["Entnahmedatum"].ToString().Should().Match("??.??.????");
            export.Rows[0]["Stationstyp"].Should().Be("NS");
            export.Rows[0]["Station"].Should().Be("anästhesiologisch");
            export.Rows[0]["Spezies"].Should().Be("Candida dubliniensis");
            export.Rows[0]["Material"].Should().Be("Isolat");
            export.Rows[0]["Labnr. Einsender"].Should().Be("LabNr. 123");
        }

        private SentinelEntryExportDefinition CreateExportDefinition()
        {
            return new SentinelEntryExportDefinition();
        }
    }
}