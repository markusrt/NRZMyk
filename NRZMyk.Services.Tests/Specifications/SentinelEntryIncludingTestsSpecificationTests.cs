using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Specifications;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Specifications;

public class SentinelEntryIncludingTestsSpecificationTests
{
    [Test]
    public void WhenSpecificationIsCreated_IncludesAreSet()
    {
        var spec = new SentinelEntryIncludingTestsSpecification(1);

        spec.IncludeExpressions.Should().Contain(
            i => i.EntityType == typeof(SentinelEntry)
                 && i.PropertyType == typeof(ICollection<AntimicrobialSensitivityTest>));
        spec.IncludeExpressions.Should().Contain(
            i => i.PreviousPropertyType == typeof(IEnumerable<AntimicrobialSensitivityTest>) 
                 && i.PropertyType == typeof(ClinicalBreakpoint));
    }

    [Test]
    public void WhenSpecificationIsCreated_ByIdFilterIsApplied()
    {
        var spec = new SentinelEntryIncludingTestsSpecification(3);

        var result = spec.Evaluate(GetTestCollection()).ToList();

        result.Should().HaveCount(1);
        result.Single().Id.Should().Be(3);
    }

    private static IEnumerable<SentinelEntry> GetTestCollection()
    {
        return new List<SentinelEntry>
        {
            new() { Id = 1, ProtectKey = "123" },
            new() { Id = 2, ProtectKey = "123" },
            new() { Id = 3, ProtectKey = "234" },
            new() { Id = 4, ProtectKey = "123" },
            new() { Id = 5, ProtectKey = "234" }
        };
    }
}