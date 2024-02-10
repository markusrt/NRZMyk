using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Specifications;

public class SentinelEntriesIncludingTestsSpecificationTests
{
    [Test]
    public void WhenSpecificationIsCreated_IncludesAreSet()
    {
        var spec = new SentinelEntriesIncludingTestsSpecification();

        spec.IncludeExpressions.Should().Contain(
            i => i.EntityType == typeof(SentinelEntry)
                 && i.PropertyType == typeof(ICollection<AntimicrobialSensitivityTest>));
        spec.IncludeExpressions.Should().Contain(
            i => i.PreviousPropertyType == typeof(IEnumerable<AntimicrobialSensitivityTest>) 
                 && i.PropertyType == typeof(ClinicalBreakpoint));
    }

}