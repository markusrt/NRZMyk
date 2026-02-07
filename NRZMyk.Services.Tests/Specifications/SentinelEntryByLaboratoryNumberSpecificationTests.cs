using FluentAssertions;
using NRZMyk.Services.Specifications;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Specifications;

public class SentinelEntryByLaboratoryNumberSpecificationTests
{
    [TestCase("SN-2011-0233", 2011, 233)]
    [TestCase("SN-2022-0236", 2022, 236)]
    public void Ctor_ParsesLaboratoryNumber(string laboratoryNumber, int year, int sequentialNumber)
    {
        var sut = new SentinelEntryByLaboratoryNumberSpecification(laboratoryNumber, "");

        sut.Year.Should().Be(year);
        sut.SequentialNumber.Should().Be(sequentialNumber);
    }

    [TestCase("")]
    [TestCase("SN-abc-0233")]
    [TestCase("SN-2010-233")]
    public void CtorInvalidNumber_UsesDefault(string laboratoryNumber)
    {
        var sut = new SentinelEntryByLaboratoryNumberSpecification(laboratoryNumber, "");

        sut.Year.Should().Be(0);
        sut.SequentialNumber.Should().Be(0);
    }
}