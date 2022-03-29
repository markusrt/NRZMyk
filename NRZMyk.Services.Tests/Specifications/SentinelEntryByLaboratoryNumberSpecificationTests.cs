using FluentAssertions;
using NRZMyk.Services.Specifications;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Specifications;

public class SentinelEntryByLaboratoryNumberSpecificationTests
{
    [Test]
    public void Ctor_ParsesLaboratoryNumber()
    {
        var sut = new SentinelEntryByLaboratoryNumberSpecification("SN-2011-0233", "");

        sut.Year.Should().Be(2011);
        sut.SequentialNumber.Should().Be(233);
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