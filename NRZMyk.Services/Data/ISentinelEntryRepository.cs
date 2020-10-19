using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;

namespace NRZMyk.Services.Data
{
    public interface ISentinelEntryRepository : IAsyncRepository<SentinelEntry>
    {
        Task<List<string>> Other(Expression<Func<SentinelEntry, string>> otherField);
        void AssignNextEntryNumber(SentinelEntry entry);
        void AssignNextCryoBoxNumber(SentinelEntry entry);
    }
}