using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;
using NUnit.Framework;
using PublicApiIntegrationTests;

namespace Api.Integration.Tests.SentinelEntries
{
    public class ListPagedWithSearchTests
    {
        [Test]
        public async Task WhenSearchTermMatchesSenderLaboratoryNumber_ReturnsFilteredResults()
        {
            var createRequest1 = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            createRequest1.SenderLaboratoryNumber = "SEARCH-123";

            var createRequest2 = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            createRequest2.SenderLaboratoryNumber = "OTHER-456";

            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest1);
            await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest2);

            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=SEARCH-123");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().HaveCount(1);
            result.SentinelEntries[0].SenderLaboratoryNumber.Should().Be("SEARCH-123");
        }

        [Test]
        public async Task WhenRequestingPageIndex_ReturnsCorrectPage()
        {
            var httpClient = ClientFactory.CreateClient();

            for (int i = 0; i < 5; i++)
            {
                var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
                createRequest.SenderLaboratoryNumber = $"ENTRY-{i:000}";
                await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);
            }

            var response1 = await httpClient.GetAsync("api/sentinel-entries?PageSize=2&PageIndex=0");
            var result1 = await response1.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();

            var response2 = await httpClient.GetAsync("api/sentinel-entries?PageSize=2&PageIndex=1");
            var result2 = await response2.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();

            response1.IsSuccessStatusCode.Should().BeTrue();
            response2.IsSuccessStatusCode.Should().BeTrue();

            result1.Should().NotBeNull();
            result1.SentinelEntries.Should().HaveCount(2);
            result1.PageCount.Should().BeGreaterThan(1);

            result2.Should().NotBeNull();
            result2.SentinelEntries.Should().HaveCountGreaterThan(0);

            result1.SentinelEntries[0].Id.Should().NotBe(result2.SentinelEntries[0].Id);
        }

        [Test]
        public async Task WhenNoSearchTermProvided_ReturnsAllResults()
        {
            var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);

            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task WhenSearchTermIsEmpty_ReturnsAllResults()
        {
            var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);

            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task WhenSearchTermMatchesSpeciesEnumDescription_ReturnsFilteredResults()
        {
            var matching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            matching.IdentifiedSpecies = Species.CandidaGlabrata;
            matching.SenderLaboratoryNumber = "SPECIES-MATCH-1";

            var nonMatching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            nonMatching.IdentifiedSpecies = Species.CandidaAlbicans;
            nonMatching.SenderLaboratoryNumber = "SPECIES-OTHER-1";

            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", matching);
            await httpClient.PostAsJsonAsync("api/sentinel-entries", nonMatching);

            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=glabrata");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().Contain(e => e.SenderLaboratoryNumber == "SPECIES-MATCH-1");
            result.SentinelEntries.Should().NotContain(e => e.SenderLaboratoryNumber == "SPECIES-OTHER-1");
        }

        [Test]
        public async Task WhenSearchTermMatchesMaterialEnumDescription_ReturnsFilteredResults()
        {
            var matching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            matching.Material = Material.PeripheralBloodCulture;
            matching.SenderLaboratoryNumber = "MATERIAL-MATCH-1";

            var nonMatching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            nonMatching.Material = Material.CentralBloodCulturePort;
            nonMatching.SenderLaboratoryNumber = "MATERIAL-OTHER-1";

            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", matching);
            await httpClient.PostAsJsonAsync("api/sentinel-entries", nonMatching);

            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=peripher");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().Contain(e => e.SenderLaboratoryNumber == "MATERIAL-MATCH-1");
            result.SentinelEntries.Should().NotContain(e => e.SenderLaboratoryNumber == "MATERIAL-OTHER-1");
        }

        [Test]
        public async Task WhenSearchTermMatchesAgeGroupDescription_ReturnsFilteredResults()
        {
            var matching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            matching.AgeGroup = AgeGroup.FortyOneToFortyFive;
            matching.SenderLaboratoryNumber = "AGE-MATCH-1";

            var nonMatching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            nonMatching.AgeGroup = AgeGroup.SixteenToTwenty;
            nonMatching.SenderLaboratoryNumber = "AGE-OTHER-1";

            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", matching);
            await httpClient.PostAsJsonAsync("api/sentinel-entries", nonMatching);

            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=41-45");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().Contain(e => e.SenderLaboratoryNumber == "AGE-MATCH-1");
            result.SentinelEntries.Should().NotContain(e => e.SenderLaboratoryNumber == "AGE-OTHER-1");
        }

        [Test]
        public async Task WhenSearchTermMatchesHospitalDepartmentDescription_ReturnsFilteredResults()
        {
            var matching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            matching.HospitalDepartment = HospitalDepartment.Neurology;
            matching.SenderLaboratoryNumber = "DEPT-MATCH-1";

            var nonMatching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            nonMatching.HospitalDepartment = HospitalDepartment.Urology;
            nonMatching.SenderLaboratoryNumber = "DEPT-OTHER-1";

            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", matching);
            await httpClient.PostAsJsonAsync("api/sentinel-entries", nonMatching);

            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=neurolog");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().Contain(e => e.SenderLaboratoryNumber == "DEPT-MATCH-1");
            result.SentinelEntries.Should().NotContain(e => e.SenderLaboratoryNumber == "DEPT-OTHER-1");
        }

        [Test]
        public async Task WhenSearchTermMatchesInternalHospitalDepartmentDescription_ReturnsFilteredResults()
        {
            var matching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            matching.HospitalDepartment = HospitalDepartment.Internal;
            matching.HospitalDepartmentType = HospitalDepartmentType.NormalUnit;
            matching.InternalHospitalDepartmentType = InternalHospitalDepartmentType.Cardiological;
            matching.SenderLaboratoryNumber = "INTDEPT-MATCH-1";

            var nonMatching = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            nonMatching.InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment;
            nonMatching.SenderLaboratoryNumber = "INTDEPT-OTHER-1";

            var httpClient = ClientFactory.CreateClient();
            await httpClient.PostAsJsonAsync("api/sentinel-entries", matching);
            await httpClient.PostAsJsonAsync("api/sentinel-entries", nonMatching);

            var response = await httpClient.GetAsync("api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=kardiologisch");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().Contain(e => e.SenderLaboratoryNumber == "INTDEPT-MATCH-1");
            result.SentinelEntries.Should().NotContain(e => e.SenderLaboratoryNumber == "INTDEPT-OTHER-1");
        }

        [Test]
        public async Task WhenSearchTermMatchesFullSentinelLaboratoryNumber_ReturnsSingleMatch()
        {
            var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            createRequest.SenderLaboratoryNumber = "SN-LAB-FULL";

            var httpClient = ClientFactory.CreateClient();
            var createResponse = await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<SentinelEntry>();
            created.Should().NotBeNull();

            var searchTerm = $"SN-{created.Year}-{created.YearlySequentialEntryNumber:0000}";
            var response = await httpClient.GetAsync(
                $"api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm={searchTerm}");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().ContainSingle()
                .Which.Id.Should().Be(created.Id);
        }

        [Test]
        public async Task WhenSearchTermMatchesSentinelLaboratoryNumberSequenceOnly_ReturnsMatch()
        {
            var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            createRequest.SenderLaboratoryNumber = "SN-LAB-SEQ-ONLY";

            var httpClient = ClientFactory.CreateClient();
            var createResponse = await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<SentinelEntry>();
            created.Should().NotBeNull();

            var sequence = created.YearlySequentialEntryNumber.ToString("0000");
            var response = await httpClient.GetAsync(
                $"api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm={sequence}");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().Contain(e => e.Id == created.Id);
        }

        [Test]
        public async Task WhenSearchTermMatchesSentinelLaboratoryNumberYearOnly_ReturnsMatch()
        {
            var createRequest = SentinelEntryTestHelper.CreateValidSentinelEntryRequest();
            createRequest.SenderLaboratoryNumber = "SN-LAB-YEAR-ONLY";

            var httpClient = ClientFactory.CreateClient();
            var createResponse = await httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<SentinelEntry>();
            created.Should().NotBeNull();

            var response = await httpClient.GetAsync(
                $"api/sentinel-entries?PageSize=50&PageIndex=0&SearchTerm=SN-{created.Year}");

            response.IsSuccessStatusCode.Should().BeTrue();
            var result = await response.Content.ReadFromJsonAsync<ListPagedSentinelEntryResponse>();
            result.Should().NotBeNull();
            result.SentinelEntries.Should().Contain(e => e.Id == created.Id);
        }
    }
}
