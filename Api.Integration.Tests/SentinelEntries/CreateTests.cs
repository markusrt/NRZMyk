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

            response.StatusCode.Should().Be(HttpStatusCode.Created);
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
            return request;
        }
    }
}
