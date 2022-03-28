using System;
using System.Net;
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
            var request = CreateValidRequest();

            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);
            var createdEntryPath = response.Headers.Location?.AbsolutePath;
            var test = await client.GetAsync(createdEntryPath);
            var createdEntry = await client.GetFromJsonAsync<SentinelEntryResponse>(createdEntryPath).ConfigureAwait(true);
            
            createdEntry.Id.Should().BeGreaterThan(0);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task WhenCreatingWithValidPredecessorEntry_RespondsWithCreate()
        {
            var client = ClientFactory.CreateClient();
            var predecessor = CreateValidRequest();

            var predecessorResponse = await client.PostAsJsonAsync("api/sentinel-entries", predecessor).ConfigureAwait(true);
            var predecessorPath = predecessorResponse.Headers.Location?.AbsolutePath;
            var predecessorEntry = await client.GetFromJsonAsync<SentinelEntryResponse>(predecessorPath).ConfigureAwait(true);
            predecessorEntry.Id.Should().BeGreaterThan(0);
            
            //Follow-up entry
            var followUp = CreateValidRequest();
            followUp.PredecessorLaboratoryNumber = predecessorEntry.LaboratoryNumber;
            var followUpResponse = await client.PostAsJsonAsync("api/sentinel-entries", followUp).ConfigureAwait(true);
            var followUpPath = followUpResponse.Headers.Location?.AbsolutePath;
            var followUpEntry = await client.GetFromJsonAsync<SentinelEntryResponse>(followUpPath).ConfigureAwait(true);

            followUpEntry.PredecessorLaboratoryNumber.Should().Be(predecessorEntry.LaboratoryNumber);
        }

        
        [Test]
        public async Task WhenCreatingPredecessorCircle_RespondsWithCreate()
        {
            var client = ClientFactory.CreateClient();
            var predecessor = CreateValidRequest();

            // Arrange: Create entry with follow-up
            var predecessorResponse = await client.PostAsJsonAsync("api/sentinel-entries", predecessor).ConfigureAwait(true);
            var predecessorPath = predecessorResponse.Headers.Location?.AbsolutePath;
            var predecessorEntry = await client.GetFromJsonAsync<SentinelEntryResponse>(predecessorPath).ConfigureAwait(true);
            predecessorEntry.Id.Should().BeGreaterThan(0);
            var followUp = CreateValidRequest();
            followUp.PredecessorLaboratoryNumber = predecessorEntry.LaboratoryNumber;
            var followUpResponse = await client.PostAsJsonAsync("api/sentinel-entries", followUp).ConfigureAwait(true);
            var followUpPath = followUpResponse.Headers.Location?.AbsolutePath;
            var followUpEntry = await client.GetFromJsonAsync<SentinelEntryResponse>(followUpPath).ConfigureAwait(true);

            // Arrange: Make original entry reference follow-up, thus creating a circle
            predecessor.Id = predecessorEntry.Id;
            predecessor.PredecessorLaboratoryNumber = followUpEntry.LaboratoryNumber;
            
            var circleResponse = await client.PutAsJsonAsync("api/sentinel-entries", predecessor).ConfigureAwait(true);
            
            circleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            predecessorEntry = await client.GetFromJsonAsync<SentinelEntryResponse>(predecessorPath).ConfigureAwait(true);
            predecessorEntry.PredecessorLaboratoryNumber.Should().Be(followUpEntry.LaboratoryNumber);
        }

        [Test]
        public async Task WhenCreatingUnknownPredecessorEntry_RespondsWithBadRequest()
        {
            var client = ClientFactory.CreateClient();
            var request = CreateValidRequest();
            request.PredecessorLaboratoryNumber = "SN-4242-0042";

            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("PredecessorLaboratoryNumber");
            content.Should().Contain("Laboratory number can not be found");
        }

        [Test]
        public async Task WhenCreatingWithMissingSamplingDate_RespondsWithBadRequest()
        {
            var client = ClientFactory.CreateClient();
            var request = CreateValidRequest();
            request.SamplingDate = null;

            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(nameof(SentinelEntryRequest.SamplingDate));
        }

        [Test]
        public async Task WhenCreatingWithFutureSamplingDate_RespondsWithBadRequest()
        {
            var client = ClientFactory.CreateClient();
            var request = CreateValidRequest();
            request.SamplingDate = DateTime.Now.AddDays(1);

            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(nameof(SentinelEntryRequest.SamplingDate));
        }

        private SentinelEntryRequest CreateValidRequest()
        {
            var filler = new Filler<SentinelEntryRequest>();
            var request = filler.Create();
            request.Material = Material.CentralBloodCultureOther;
            request.HospitalDepartment = HospitalDepartment.GeneralSurgery;
            request.IdentifiedSpecies = Species.CandidaDubliniensis;
            request.SpeciesIdentificationMethod = SpeciesIdentificationMethod.BBL;
            request.SamplingDate = DateTime.Now.AddDays(-3);
            request.PredecessorLaboratoryNumber = string.Empty;
            request.HasPredecessor = YesNo.No;
            return request;
        }
    }
}
