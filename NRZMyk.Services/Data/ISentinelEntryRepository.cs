using System;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;

namespace NRZMyk.Services.Data
{
    public interface ISentinelEntryRepository : IAsyncRepository<SentinelEntry>
    {
        void AssignNextEntryNumber(SentinelEntry entry);
        void AssignNextCryoBoxNumber(SentinelEntry entry);
    }
}