using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public class SentinelEntryServiceImpl : ISentinelEntryService
    {
        private const string BaseApi = "api/sentinel-entries";
        private readonly IHttpClient _httpClient;

        public SentinelEntryServiceImpl(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<SentinelEntry> Create(SentinelEntryRequest createRequest)
            => _httpClient.Post<SentinelEntryRequest, SentinelEntry>(BaseApi, createRequest);

        public Task<SentinelEntry> Update(SentinelEntryRequest updateRequest)
            => _httpClient.Put<SentinelEntryRequest, SentinelEntry>(BaseApi, updateRequest);

        public Task<SentinelEntry> CryoArchive(CryoArchiveRequest archiveRequest)
            => _httpClient.Put<CryoArchiveRequest, SentinelEntry>($"{BaseApi}/cryo-archive", archiveRequest);

        public Task<SentinelEntry> UpdateCryoRemark(CryoRemarkUpdateRequest updateRequest)
            => _httpClient.Put<CryoRemarkUpdateRequest, SentinelEntry>($"{BaseApi}/cryo-remark-update", updateRequest);

        public Task<string> Export()
            => _httpClient.GetBytesAsBase64($"{BaseApi}/export");

        public Task<List<string>> Other(string other) 
            => _httpClient.Get<List<string>>($"{BaseApi}/other/{other}");

        public Task Delete(int id)
            => _httpClient.Delete<SentinelEntry>($"{BaseApi}/{id}");

        public Task<List<SentinelEntry>> ListByOrganization(int organizationId)
            => _httpClient.Get<List<SentinelEntry>>($"{BaseApi}/organization/{organizationId}");

        public Task<SentinelEntryResponse> GetById(int id)
            => _httpClient.Get<SentinelEntryResponse>($"{BaseApi}/{id}");

        public async Task<List<SentinelEntry>> ListPaged(int pageSize)
        {
            var pagedResult = await _httpClient
                .Get<PagedSentinelEntryResult>($"{BaseApi}?PageSize={pageSize}").ConfigureAwait(false);
            return pagedResult.SentinelEntries;
        }

        public async Task<PagedSentinelEntryResult> ListPaged(int pageSize, int pageIndex, string searchTerm = null, int? organizationId = null)
        {
            var queryParams = new List<string>
            {
                $"PageSize={pageSize}",
                $"PageIndex={pageIndex}"
            };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                queryParams.Add($"SearchTerm={Uri.EscapeDataString(searchTerm)}");
            }

            if (organizationId.HasValue)
            {
                queryParams.Add($"OrganizationId={organizationId.Value}");
            }

            var queryString = string.Join("&", queryParams);
            return await _httpClient
                .Get<PagedSentinelEntryResult>($"{BaseApi}?{queryString}").ConfigureAwait(false);
        }
    }
}