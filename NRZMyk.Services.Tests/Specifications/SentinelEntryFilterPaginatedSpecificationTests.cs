using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Specifications;

public class SentinelEntryFilterPaginatedSpecificationTests
{
    [Test]
    public void WhenSpecificationIsCreated_FiltersByProtectKey()
    {
        var spec = new SentinelEntryFilterPaginatedSpecification(0, 10, "123");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(3);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersByProtectKeyAndMatchesPageSizer()
    {
        var spec = new SentinelEntryFilterPaginatedSpecification(0, 2, "123");

        var result = spec.Evaluate(GetTestCollection());

        result.Should().HaveCount(2);
    }

    [Test]
    public void WhenSpecificationIsCreated_FiltersByProtectKeyAndSkipsEntries()
    {
        var spec = new SentinelEntryFilterPaginatedSpecification(1, 20, "234");

        var result = spec.Evaluate(GetTestCollection()).ToList();

        result.Should().HaveCount(1);
        result.Single().Id.Should().Be(3);
    }

    [Test]
    public void WhenSpecificationIsCreated_OrdersByIdDescending()
    {
        var spec = new SentinelEntryFilterPaginatedSpecification(0, 20, "123");

        var result = spec.Evaluate(GetTestCollection());

        result.Select(s => s.Id).Should().ContainInConsecutiveOrder(new List<int>{4,2,1});
    }

    private static IEnumerable<SentinelEntry> GetTestCollection()
    {
        return new List<SentinelEntry>
        {
            new() {Id = 1, ProtectKey = "123"},
            new() {Id = 2, ProtectKey = "123"},
            new() {Id = 3, ProtectKey = "234"},
            new() {Id = 4, ProtectKey = "123"},
            new() {Id = 5, ProtectKey = "234"}
        };
    }
}