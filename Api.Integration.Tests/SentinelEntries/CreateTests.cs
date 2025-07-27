using System;
using System.Diagnostics.CodeAnalysis;
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
    public class CreateTests
    {
        [Test]
        public async Task WhenCreatingValidSentinelEntry_RespondsWithCreate()
        {
            var client = ClientFactory.CreateClient();
            var request = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();

            var createdEntry = await CreateValidEntry(client, request).ConfigureAwait(true);

            createdEntry.Should().NotBeNull();
            createdEntry!.Id.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task WhenCreatingWithValidPredecessorEntry_RespondsWithCreate()
        {
            var client = ClientFactory.CreateClient();
            var predecessor = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();

            var predecessorEntry = await CreateValidEntry(client, predecessor).ConfigureAwait(true);
            
            //Follow-up entry
            var followUp = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            followUp.PredecessorLaboratoryNumber = predecessorEntry!.LaboratoryNumber;
            var followUpEntry = await CreateValidEntry(client, followUp).ConfigureAwait(true);

            followUpEntry!.PredecessorLaboratoryNumber.Should().Be(predecessorEntry!.LaboratoryNumber);
        }

        
        [Test]
        public async Task WhenCreatingPredecessorCircle_RespondsWithCreate()
        {
            var client = ClientFactory.CreateClient();
            var predecessorRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();

            // Arrange: Create entry with follow-up
            var predecessor = await CreateValidEntry(client, predecessorRequest).ConfigureAwait(true);
            predecessor.Should().NotBeNull();
            predecessor!.Id.Should().BeGreaterThan(0);
            var followUp = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            followUp.PredecessorLaboratoryNumber = predecessor!.LaboratoryNumber;
            var followUpEntry = await CreateValidEntry(client, followUp).ConfigureAwait(true);
            followUpEntry.Should().NotBeNull();

            // Arrange: Make original entry reference follow-up, thus creating a circle
            predecessorRequest.Id = predecessor!.Id;
            predecessorRequest.PredecessorLaboratoryNumber = followUpEntry!.LaboratoryNumber;
            
            var circleResponse = await client.PutAsJsonAsync("api/sentinel-entries", predecessorRequest).ConfigureAwait(true);
            
            circleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            predecessor = await client.GetFromJsonAsync<SentinelEntryResponse>($"api/sentinel-entries/{predecessor.Id}").ConfigureAwait(true);
            predecessor.PredecessorLaboratoryNumber.Should().Be(followUpEntry!.LaboratoryNumber);
        }


        [Test]
        public async Task WhenCreatingUnknownPredecessorEntry_RespondsWithBadRequest()
        {
            var client = ClientFactory.CreateClient();
            var request = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            request.PredecessorLaboratoryNumber = "SN-4242-0042";

            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            content.Should().Contain("PredecessorLaboratoryNumber");
            content.Should().Contain("Laboratory number can not be found");
        }

        [Test]
        public async Task WhenCreatingWithMissingSamplingDate_RespondsWithBadRequest()
        {
            var client = ClientFactory.CreateClient();
            var request = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            request.SamplingDate = null;

            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(nameof(SentinelEntryRequest.SamplingDate));
        }

        [Test]
        public async Task WhenCreatingWithInvalidDepartmentCombination_RespondsWithBadRequest()
        {
            var client = ClientFactory.CreateClient();
            var request = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            request.InternalHospitalDepartmentType = InternalHospitalDepartmentType.Angiological;

            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            content.Should().Contain(nameof(SentinelEntryRequest.InternalHospitalDepartmentType));
        }

        [Test]
        public async Task WhenCreatingWithFutureSamplingDate_RespondsWithBadRequest()
        {
            var client = ClientFactory.CreateClient();
            var request = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            request.SamplingDate = DateTime.Now.AddDays(1);

            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(nameof(SentinelEntryRequest.SamplingDate));
        }
        
        private static async Task<SentinelEntryResponse?> CreateValidEntry(HttpClient client, SentinelEntryRequest predecessor)
        {
            var response = await client.PostAsJsonAsync("api/sentinel-entries", predecessor).ConfigureAwait(true);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdEntryPath = response.Headers.Location?.AbsolutePath;
            var createdEntry = await client.GetFromJsonAsync<SentinelEntryResponse>(createdEntryPath).ConfigureAwait(true);
            createdEntry!.Id.Should().BeGreaterThan(0);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            return createdEntry;
        }
    }
}
