using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Specifications;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Specifications;

public class ClinicalBreakpointFilterSpecificationTests
{
    [TestCase(Species.CandidaAlbicans, 1)]
    [TestCase(Species.CandidaDubliniensis, 2)]
    [TestCase(null, 4)]
    public void WhenSpecificationIsCreated_SpeciesFilterIsApplied(Species? species, int expectedCount)
    {
        var spec = new ClinicalBreakpointFilterSpecification(species);

        var result = spec.Evaluate(GetTestItemCollection()).ToList();

        result.Should().HaveCount(expectedCount);
    }

    private static IEnumerable<ClinicalBreakpoint> GetTestItemCollection()
    {
        return new List<ClinicalBreakpoint>
        {
            new() {Species = Species.CandidaAlbicans},
            new() {Species = Species.CandidaDubliniensis},
            new() {Species = Species.CandidaDubliniensis},
            new() {Species = Species.CandidaGlabrata}
        };
    }
}