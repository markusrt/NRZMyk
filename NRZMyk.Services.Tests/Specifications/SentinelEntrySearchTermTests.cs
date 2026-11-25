using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Specifications;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Specifications;

public class SentinelEntrySearchTermTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Parse_WithBlankInput_ReturnsEmpty(string input)
    {
        var result = SentinelEntrySearchTerm.Parse(input);

        result.IsEmpty.Should().BeTrue();
    }

    [Test]
    public void Parse_NormalizesTermToLowerCaseAndTrimmed()
    {
        var result = SentinelEntrySearchTerm.Parse("  Candida ALBicans  ");

        result.IsEmpty.Should().BeFalse();
        result.NormalizedTerm.Should().Be("candida albicans");
    }

    [Test]
    public void Parse_MatchesMaterialEnumByDescription()
    {
        var result = SentinelEntrySearchTerm.Parse("Blutkultur peripher");

        result.MaterialMatches.Should().Contain(Material.PeripheralBloodCulture);
        // Substring match should not pull other Blutkultur values when the term is exact
        result.MaterialMatches.Should().NotContain(Material.CentralBloodCultureCvc);
    }

    [Test]
    public void Parse_MatchesAllMaterialsContainingSubstring()
    {
        var result = SentinelEntrySearchTerm.Parse("Blutkultur");

        result.MaterialMatches.Should().Contain(new[]
        {
            Material.PeripheralBloodCulture,
            Material.CentralBloodCultureCvc,
            Material.CentralBloodCulturePort,
            Material.CentralBloodCultureShaldon,
            Material.CentralBloodCultureOther,
            Material.BloodCultureOther
        });
    }

    [Test]
    public void Parse_MatchesAgeGroupByDescription()
    {
        var result = SentinelEntrySearchTerm.Parse("41-45");

        result.AgeGroupMatches.Should().Contain(AgeGroup.FortyOneToFortyFive);
    }

    [Test]
    public void Parse_MatchesSpeciesByDescription()
    {
        var result = SentinelEntrySearchTerm.Parse("glabrata");

        result.SpeciesMatches.Should().ContainSingle().Which.Should().Be(Species.CandidaGlabrata);
    }

    [Test]
    public void Parse_MatchesHospitalDepartmentByDescription()
    {
        var result = SentinelEntrySearchTerm.Parse("neurolog");

        result.HospitalDepartmentMatches.Should().Contain(HospitalDepartment.Neurology);
    }

    [Test]
    public void Parse_MatchesInternalHospitalDepartmentByDescription()
    {
        var result = SentinelEntrySearchTerm.Parse("kardiologisch");

        result.InternalHospitalDepartmentMatches.Should()
            .Contain(InternalHospitalDepartmentType.Cardiological);
    }

    [Test]
    public void Parse_FullSentinelLaboratoryNumber_SetsExactYearAndSequence()
    {
        var result = SentinelEntrySearchTerm.Parse("SN-2022-0004");

        result.ExactYear.Should().Be(2022);
        result.ExactSequenceNumber.Should().Be(4);
        result.CandidateYear.Should().BeNull();
        result.CandidateSequenceNumber.Should().BeNull();
    }

    [Test]
    public void Parse_LaboratoryNumberWithoutPrefix_SetsExactYearAndSequence()
    {
        var result = SentinelEntrySearchTerm.Parse("2022-0004");

        result.ExactYear.Should().Be(2022);
        result.ExactSequenceNumber.Should().Be(4);
    }

    [Test]
    public void Parse_LaboratoryNumberWithPrefixOnly_SetsCandidateYear()
    {
        var result = SentinelEntrySearchTerm.Parse("SN-2024");

        result.CandidateYear.Should().Be(2024);
        result.CandidateSequenceNumber.Should().BeNull();
        result.ExactYear.Should().BeNull();
    }

    [Test]
    public void Parse_FourDigitsAlone_SetsBothYearAndSequenceCandidate()
    {
        var result = SentinelEntrySearchTerm.Parse("2024");

        result.CandidateYear.Should().Be(2024);
        result.CandidateSequenceNumber.Should().Be(2024);
    }

    [Test]
    public void Parse_TwoDigitsAlone_SetsOnlySequenceCandidate()
    {
        var result = SentinelEntrySearchTerm.Parse("12");

        result.CandidateYear.Should().BeNull();
        result.CandidateSequenceNumber.Should().Be(12);
    }

    [Test]
    public void Parse_FreeTextWithoutEnumOrLaboratoryNumber_LeavesOnlyNormalizedTerm()
    {
        var result = SentinelEntrySearchTerm.Parse("ABCXYZ");

        result.NormalizedTerm.Should().Be("abcxyz");
        result.MaterialMatches.Should().BeEmpty();
        result.AgeGroupMatches.Should().BeEmpty();
        result.SpeciesMatches.Should().BeEmpty();
        result.HospitalDepartmentMatches.Should().BeEmpty();
        result.InternalHospitalDepartmentMatches.Should().BeEmpty();
        result.ExactYear.Should().BeNull();
        result.ExactSequenceNumber.Should().BeNull();
        result.CandidateYear.Should().BeNull();
        result.CandidateSequenceNumber.Should().BeNull();
    }
}
