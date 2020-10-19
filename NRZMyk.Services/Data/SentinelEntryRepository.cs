using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Data
{
    public class SentinelEntryRepository : EfRepository<SentinelEntry>, ISentinelEntryRepository
    {
        private readonly int _maxSize;
        private static int Default10x10CryoBoxSize = 10*10;

        public SentinelEntryRepository(IOptions<ApplicationSettings> config, ApplicationDbContext dbContext) : base(dbContext)
        {
            _maxSize = config?.Value?.Application?.CryoBoxSize ?? Default10x10CryoBoxSize;
        }

        public Task<List<string>> Other(Expression<Func<SentinelEntry, string>> otherField)
        {
            return _dbContext.SentinelEntries.Select(otherField).Distinct()
                .Where(s => s != null).OrderBy(s => s).ToListAsync();
        }

        public void AssignNextEntryNumber(SentinelEntry entry)
        {
            var currentYear = DateTime.Now.Year;
            var lastSequentialIsolateNumber =
                _dbContext.SentinelEntries.Where(i => i.Year == currentYear)
                    .DefaultIfEmpty()
                    .Max(i => i == null ? 0 : i.YearlySequentialEntryNumber);

            entry.Year = currentYear;
            entry.YearlySequentialEntryNumber = lastSequentialIsolateNumber + 1;
        }

        public void AssignNextCryoBoxNumber(SentinelEntry entry)
        {
            var currentYear = DateTime.Now.Year;
            entry.Year = currentYear;
            var currentBoxNumber = _dbContext.SentinelEntries.Where(i => i.Year == currentYear)
                .DefaultIfEmpty().Max(s => s == null ? 1 : s.CryoBoxNumber);
            var currentBoxSlot = _dbContext.SentinelEntries.Where(
                    s =>  s.Year == currentYear && s.CryoBoxNumber == currentBoxNumber)
                .DefaultIfEmpty().Max(s => s == null ? 0 : s.CryoBoxSlot);

            var currentBoxFull = currentBoxSlot == _maxSize;

            entry.CryoBoxNumber = currentBoxFull ? currentBoxNumber + 1 : currentBoxNumber;
            entry.CryoBoxSlot = currentBoxFull ? 1 : currentBoxSlot + 1;
        }
    }
}