using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Specifications;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Specifications;

public class SentinelEntrySearchFilterSpecificationTests
{
    [Test]
    public void WhenSpecificationIsCreated_FiltersByProtectKey()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(4);
        result.Select(s => s.Id).Should().ContainInConsecutiveOrder(new List<int> { 6, 4, 2, 1 });
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersBySearchTerm_SenderLaboratoryNumber()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "SENDER-123");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(1);
        result.Single().Id.Should().Be(1);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersBySearchTerm_OtherIdentifiedSpecies()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "Custom Species");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(1);
        result.Single().Id.Should().Be(4);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersBySearchTerm_OtherMaterial()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "MyOwnMaterial");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(1);
        result.Single().Id.Should().Be(4);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersBySearchTerm_OtherHospitalDepartment()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "MyOwnDepartment");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(1);
        result.Single().Id.Should().Be(4);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersBySearchTerm_CaseInsensitive()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "sender-123");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(1);
        result.Single().Id.Should().Be(1);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersByMaterialEnumDescription()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "Blutkultur peripher");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().ContainSingle().Which.Id.Should().Be(1);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersByAgeGroupEnumDescription()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "41-45");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().ContainSingle().Which.Id.Should().Be(2);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersBySpeciesEnumDescription()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "glabrata");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().ContainSingle().Which.Id.Should().Be(1);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersByHospitalDepartmentEnumDescription()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "neurolog");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().ContainSingle().Which.Id.Should().Be(2);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersByInternalHospitalDepartmentEnumDescription()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "kardiologisch");

        var result = spec.Evaluate(GetTestCollection());

        result.Select(s => s.Id).Should().BeEquivalentTo(new[] { 1, 6 });
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersByFullSentinelLaboratoryNumber()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "SN-2020-0001");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().ContainSingle().Which.Id.Should().Be(1);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersBySentinelLaboratoryNumberWithoutPrefix()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "2020-0004");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().ContainSingle().Which.Id.Should().Be(4);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersByYearOnlyLaboratoryNumber()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "SN-2021");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().ContainSingle().Which.Id.Should().Be(6);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersBySequenceOnlyLaboratoryNumber()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "0004");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().ContainSingle().Which.Id.Should().Be(4);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersByProtectKeyAndSearchTerm_WithUniqueSearchTerm()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", "Custom Species");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(1);
        result.Single().Id.Should().Be(4);
    }

    [Test]
    public void WhenSpecificationIsCreated_WithoutProtectKey_ReturnsAllMatchingSearch()
    {
        var spec = new SentinelEntrySearchFilterSpecification(null, "SENDER");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(6);
    }

    [Test]
    public void WhenSpecificationIsCreated_WithoutSearchTerm_ReturnsAllInOrganization()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123", null);

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(4);
    }

    [Test]
    public void WhenSpecificationIsCreated_OrdersByIdDescending()
    {
        var spec = new SentinelEntrySearchFilterSpecification("123");

        var result = spec.Evaluate(GetTestCollection());

        result.Select(s => s.Id).Should().ContainInConsecutiveOrder(new List<int> { 6, 4, 2, 1 });
    }

    private static IEnumerable<SentinelEntry> GetTestCollection()
    {
        return new List<SentinelEntry>
        {
            new() {
                Id = 1,
                ProtectKey = "123",
                Year = 2020,
                YearlySequentialEntryNumber = 1,
                SenderLaboratoryNumber = "SENDER-123",
                SamplingDate = new DateTime(2020, 5, 1),
                Material = Material.PeripheralBloodCulture,
                AgeGroup = AgeGroup.SixteenToTwenty,
                HospitalDepartment = HospitalDepartment.Internal,
                InternalHospitalDepartmentType = InternalHospitalDepartmentType.Cardiological,
                IdentifiedSpecies = Species.CandidaGlabrata,
                OtherIdentifiedSpecies = null
            },
            new() {
                Id = 2,
                ProtectKey = "123",
                Year = 2020,
                YearlySequentialEntryNumber = 2,
                SenderLaboratoryNumber = "SENDER-456",
                SamplingDate = new DateTime(2020, 6, 1),
                Material = Material.CentralBloodCultureCvc,
                AgeGroup = AgeGroup.FortyOneToFortyFive,
                HospitalDepartment = HospitalDepartment.Neurology,
                InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment,
                IdentifiedSpecies = Species.CandidaAlbicans,
                OtherIdentifiedSpecies = null
            },
            new() {
                Id = 3,
                ProtectKey = "234",
                Year = 2020,
                YearlySequentialEntryNumber = 3,
                SenderLaboratoryNumber = "SENDER-789",
                SamplingDate = new DateTime(2020, 7, 1),
                Material = Material.PeripheralBloodCulture,
                AgeGroup = AgeGroup.SixtyOneToSixtyFive,
                HospitalDepartment = HospitalDepartment.Anaesthetics,
                InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment,
                IdentifiedSpecies = Species.CandidaTropicalis,
                OtherIdentifiedSpecies = null
            },
            new() {
                Id = 4,
                ProtectKey = "123",
                Year = 2020,
                YearlySequentialEntryNumber = 4,
                SenderLaboratoryNumber = "SENDER-999",
                SamplingDate = new DateTime(2020, 8, 1),
                Material = Material.Other,
                OtherMaterial = "MyOwnMaterial",
                AgeGroup = AgeGroup.TwentyOneToTwentyFive,
                HospitalDepartment = HospitalDepartment.Other,
                OtherHospitalDepartment = "MyOwnDepartment",
                InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment,
                IdentifiedSpecies = Species.Other,
                OtherIdentifiedSpecies = "Custom Species"
            },
            new() {
                Id = 5,
                ProtectKey = "234",
                Year = 2020,
                YearlySequentialEntryNumber = 5,
                SenderLaboratoryNumber = "SENDER-111",
                SamplingDate = new DateTime(2020, 9, 1),
                Material = Material.PeripheralBloodCulture,
                AgeGroup = AgeGroup.SixtyOneToSixtyFive,
                HospitalDepartment = HospitalDepartment.Internal,
                InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment,
                IdentifiedSpecies = Species.CandidaParapsilosis,
                OtherIdentifiedSpecies = null
            },
            new() {
                Id = 6,
                ProtectKey = "123",
                Year = 2021,
                YearlySequentialEntryNumber = 30,
                SenderLaboratoryNumber = "SENDER-2021",
                SamplingDate = new DateTime(2021, 9, 21),
                Material = Material.BloodCultureOther,
                AgeGroup = AgeGroup.Unknown,
                HospitalDepartment = HospitalDepartment.Internal,
                InternalHospitalDepartmentType = InternalHospitalDepartmentType.Cardiological,
                IdentifiedSpecies = Species.CandidaAlbicans,
                OtherIdentifiedSpecies = null
            }
        };
    }
}
