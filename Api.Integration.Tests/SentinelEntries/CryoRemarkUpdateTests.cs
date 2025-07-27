using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NUnit.Framework;
using PublicApiIntegrationTests;
using Tynamix.ObjectFiller;

namespace Api.Integration.Tests.SentinelEntries
{
    public class CryoRemarkUpdateTests
    {
        [Test]
        public async Task WhenUpdatingCryoRemarkForExistingEntry_RespondsWithOk()
        {
            var client = ClientFactory.CreateClient();
            
            // Arrange: Create a sentinel entry first
            var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            var createdEntry = await CreateValidEntry(client, createRequest).ConfigureAwait(true);
            createdEntry.Should().NotBeNull();
            
            // Arrange: Update cryo remark
            var updateRequest = new CryoRemarkUpdateRequest
            {
                Id = createdEntry!.Id,
                CryoRemark = "Updated test remark"
            };

            // Act
            var response = await client.PutAsJsonAsync("api/sentinel-entries/cryo-remark-update", updateRequest).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedEntry = await response.Content.ReadFromJsonAsync<SentinelEntryResponse>().ConfigureAwait(true);
            updatedEntry.Should().NotBeNull();
            updatedEntry!.CryoRemark.Should().Be("Updated test remark");
            updatedEntry.Id.Should().Be(createdEntry.Id);
        }

        [Test]
        public async Task WhenUpdatingCryoRemarkForNonExistentEntry_RespondsWithNotFound()
        {
            var client = ClientFactory.CreateClient();
            
            var updateRequest = new CryoRemarkUpdateRequest
            {
                Id = 99999, // Non-existent ID
                CryoRemark = "Test remark"
            };

            var response = await client.PutAsJsonAsync("api/sentinel-entries/cryo-remark-update", updateRequest).ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task WhenUpdatingCryoRemarkOnlyUpdatesRemarkField()
        {
            var client = ClientFactory.CreateClient();
            
            // Arrange: Create a sentinel entry first
            var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            var createdEntry = await CreateValidEntry(client, createRequest).ConfigureAwait(true);
            createdEntry.Should().NotBeNull();

            // Store original values
            var originalLaboratoryNumber = createdEntry!.LaboratoryNumber;
            var originalSamplingDate = createdEntry.SamplingDate;
            var originalMaterial = createdEntry.Material;
            
            // Arrange: Update cryo remark
            var updateRequest = new CryoRemarkUpdateRequest
            {
                Id = createdEntry.Id,
                CryoRemark = "Updated remark only"
            };

            // Act
            var response = await client.PutAsJsonAsync("api/sentinel-entries/cryo-remark-update", updateRequest).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedEntry = await response.Content.ReadFromJsonAsync<SentinelEntryResponse>().ConfigureAwait(true);
            updatedEntry.Should().NotBeNull();
            
            // Verify only CryoRemark was updated
            updatedEntry!.CryoRemark.Should().Be("Updated remark only");
            updatedEntry.LaboratoryNumber.Should().Be(originalLaboratoryNumber);
            updatedEntry.SamplingDate.Should().Be(originalSamplingDate);
            updatedEntry.Material.Should().Be(originalMaterial);
        }

        private static async Task<SentinelEntryResponse?> CreateValidEntry(HttpClient client, SentinelEntryRequest request)
        {
            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdEntryPath = response.Headers.Location?.AbsolutePath;
            var createdEntry = await client.GetFromJsonAsync<SentinelEntryResponse>(createdEntryPath).ConfigureAwait(true);
            createdEntry!.Id.Should().BeGreaterThan(0);
            return createdEntry;
        }
    }
}