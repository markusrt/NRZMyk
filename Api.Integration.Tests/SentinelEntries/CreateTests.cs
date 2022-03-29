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
    public class CreateTests
    {
        [Test]
        public async Task WhenCreatingValidSentinelEntry_RespondsWithCreate()
        {
            var client = ClientFactory.CreateClient();
            var request = CreateValidRequest();

            var createdEntry = await CreateValidEntry(client, request);

            createdEntry.Should().NotBeNull();
            createdEntry.Id.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task WhenCreatingWithValidPredecessorEntry_RespondsWithCreate()
        {
            var client = ClientFactory.CreateClient();
            var predecessor = CreateValidRequest();

            var predecessorEntry = await CreateValidEntry(client, predecessor);
            
            //Follow-up entry
            var followUp = CreateValidRequest();
            followUp.PredecessorLaboratoryNumber = predecessorEntry?.LaboratoryNumber;
            var followUpEntry = await CreateValidEntry(client, followUp);

            followUpEntry?.PredecessorLaboratoryNumber.Should().Be(predecessorEntry.LaboratoryNumber);
        }

        
        [Test]
        public async Task WhenCreatingPredecessorCircle_RespondsWithCreate()
        {
            var client = ClientFactory.CreateClient();
            var predecessorRequest = CreateValidRequest();

            // Arrange: Create entry with follow-up
            var predecessor = await CreateValidEntry(client, predecessorRequest);
            predecessor.Should().NotBeNull();
            predecessor?.Id.Should().BeGreaterThan(0);
            var followUp = CreateValidRequest();
            followUp.PredecessorLaboratoryNumber = predecessor?.LaboratoryNumber;
            var followUpEntry = await CreateValidEntry(client, followUp);
            followUpEntry.Should().NotBeNull();

            // Arrange: Make original entry reference follow-up, thus creating a circle
            predecessorRequest.Id = predecessor.Id;
            predecessorRequest.PredecessorLaboratoryNumber = followUpEntry?.LaboratoryNumber;
            
            var circleResponse = await client.PutAsJsonAsync("api/sentinel-entries", predecessorRequest).ConfigureAwait(true);
            
            circleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            predecessor = await client.GetFromJsonAsync<SentinelEntryResponse>($"api/sentinel-entries/{predecessor.Id}").ConfigureAwait(true);
            predecessor?.PredecessorLaboratoryNumber.Should().Be(followUpEntry?.LaboratoryNumber);
        }


        [Test]
        public async Task WhenCreatingUnknownPredecessorEntry_RespondsWithBadRequest()
        {
            var client = ClientFactory.CreateClient();
            var request = CreateValidRequest();
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
            var request = CreateValidRequest();
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
            var request = CreateValidRequest();
            request.InternalHospitalDepartmentType = InternalHospitalDepartmentType.Angiological;

            var response = await client.PostAsJsonAsync("api/sentinel-entries", request).ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(nameof(SentinelEntryRequest.InternalHospitalDepartmentType));
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
        
        private static async Task<SentinelEntryResponse?> CreateValidEntry(HttpClient client, SentinelEntryRequest predecessor)
        {
            var response = await client.PostAsJsonAsync("api/sentinel-entries", predecessor).ConfigureAwait(true);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdEntryPath = response.Headers.Location?.AbsolutePath;
            var createdEntry = await client.GetFromJsonAsync<SentinelEntryResponse>(createdEntryPath).ConfigureAwait(true);
            createdEntry?.Id.Should().BeGreaterThan(0);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            return createdEntry;
        }

        private SentinelEntryRequest CreateValidRequest()
        {
            var filler = new Filler<SentinelEntryRequest>();
            var request = filler.Create();
            request.Material = Material.CentralBloodCultureOther;
            request.HospitalDepartment = HospitalDepartment.GeneralSurgery;
            request.InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment;
            request.IdentifiedSpecies = Species.CandidaDubliniensis;
            request.SpeciesIdentificationMethod = SpeciesIdentificationMethod.BBL;
            request.SamplingDate = DateTime.Now.AddDays(-3);
            request.PredecessorLaboratoryNumber = string.Empty;
            request.HasPredecessor = YesNo.No;
            return request;
        }
    }
}
