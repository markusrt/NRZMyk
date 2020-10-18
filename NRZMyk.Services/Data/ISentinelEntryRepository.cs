using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;

namespace NRZMyk.Services.Data
{
    public interface ISentinelEntryRepository : IAsyncRepository<SentinelEntry>
    {
        Task<List<string>> OtherMaterials();
        void AssignNextEntryNumber(SentinelEntry entry);
        void AssignNextCryoBoxNumber(SentinelEntry entry);
    }
}