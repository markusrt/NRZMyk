using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Specifications;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Specifications;

public class SentinelEntryCountSpecificationTests
{
    [Test]
    public void WhenSpecificationIsCreated_FiltersByProtectKey()
    {
        var spec = new SentinelEntryCountSpecification("123");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(4);
    }

    [Test]
    public void WhenSpecificationIsCreatedWithCryoDate_FiltersEntriesWithCryoDate()
    {
        var spec = new SentinelEntryCountSpecification("123", hasCryoDate: true);

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(2);
        result.All(s => s.CryoDate.HasValue).Should().BeTrue();
    }

    [Test]
    public void WhenSpecificationIsCreatedWithoutCryoDate_FiltersEntriesWithoutCryoDate()
    {
        var spec = new SentinelEntryCountSpecification("123", hasCryoDate: false);

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(2);
        result.All(s => !s.CryoDate.HasValue).Should().BeTrue();
    }

    [Test]
    public void WhenSpecificationIsCreatedWithYear_FiltersEntriesByYear()
    {
        var spec = new SentinelEntryCountSpecification("123", year: 2024);

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(2);
        result.All(s => s.Year == 2024).Should().BeTrue();
    }

    [Test]
    public void WhenSpecificationIsCreatedWithCryoDateAndYear_FiltersEntriesByBoth()
    {
        var spec = new SentinelEntryCountSpecification("123", hasCryoDate: true, year: 2024);

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(2);
    }

    private static IEnumerable<SentinelEntry> GetTestCollection()
    {
        return new List<SentinelEntry>
        {
            new() { Id = 1, ProtectKey = "123", CryoDate = null, Year = 2023 },
            new() { Id = 2, ProtectKey = "123", CryoDate = new DateTime(2024, 1, 15), Year = 2024 },
            new() { Id = 3, ProtectKey = "234", CryoDate = new DateTime(2024, 2, 20), Year = 2024 },
            new() { Id = 4, ProtectKey = "123", CryoDate = new DateTime(2023, 12, 10), Year = 2023 },
            new() { Id = 5, ProtectKey = "123", CryoDate = null, Year = 2024 },
            new() { Id = 6, ProtectKey = "234", CryoDate = null, Year = 2024 }
        };
    }
}
