using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Api.Integration.Tests.SentinelEntries;
using FluentAssertions;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NUnit.Framework;
using PublicApiIntegrationTests;

namespace Api.Integration.Tests.SentinelEntries
{
    public class ListPagedWithSearchTests
    {
        [Test]
        public async Task Get_WithSearchTerm_ReturnsFilteredResults()
        {
            // Arrange
            var createRequest1 = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            createRequest1.SenderLaboratoryNumber = "SEARCH-123";
            
            var createRequest2 = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            createRequest2.SenderLaboratoryNumber = "OTHER-456";

            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest1);
            await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest2);

            // Act
            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=SEARCH-123");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().HaveCount(1);
            result.SentinelEntries[0].SenderLaboratoryNumber.Should().Be("SEARCH-123");
        }

        [Test]
        public async Task Get_WithPageIndex_ReturnsCorrectPage()
        {
            // Arrange
            var httpClient = ClientFactory.CreateClient();
            
            // Create multiple entries
            for (int i = 0; i < 5; i++)
            {
                var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
                createRequest.SenderLaboratoryNumber = $"ENTRY-{i:000}";
                await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);
            }

            // Act - Get first page with page size 2
            var response1 = await httpClient.GetAsync("api/sentinel-entries?PageSize=2&PageIndex=0");
            var result1 = await response1.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();

            // Act - Get second page with page size 2
            var response2 = await httpClient.GetAsync("api/sentinel-entries?PageSize=2&PageIndex=1");
            var result2 = await response2.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();

            // Assert
            response1.IsSuccessStatusCode.Should().BeTrue();
            response2.IsSuccessStatusCode.Should().BeTrue();
            
            result1.Should().NotBeNull();
            result1.SentinelEntries.Should().HaveCount(2);
            result1.PageCount.Should().BeGreaterThan(1);

            result2.Should().NotBeNull();
            result2.SentinelEntries.Should().HaveCountGreaterThan(0);
            
            // Ensure different entries on different pages
            result1.SentinelEntries[0].Id.Should().NotBe(result2.SentinelEntries[0].Id);
        }

        [Test]
        public async Task Get_WithoutSearchTerm_ReturnsAllResults()
        {
            // Arrange
            var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);

            // Act
            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task Get_WithEmptySearchTerm_ReturnsAllResults()
        {
            // Arrange
            var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);

            // Act
            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().HaveCountGreaterThan(0);
        }
    }
}