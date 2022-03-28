using System.Collections.Generic;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services;

public interface ISentinelEntryService
{
    Task<SentinelEntry> Create(SentinelEntryRequest createRequest);
    Task<List<SentinelEntry>> ListPaged(int pageSize);
    Task<List<SentinelEntry>> ListByOrganization(int organizationId);
    Task<SentinelEntryResponse> GetById(int id);
    Task<SentinelEntry> Update(SentinelEntryRequest updateRequest);
    Task<SentinelEntry> CryoArchive(CryoArchiveRequest archiveRequest);
    Task<string> Export();
    Task<List<string>> Other(string other);
    Task Delete(int id);
}