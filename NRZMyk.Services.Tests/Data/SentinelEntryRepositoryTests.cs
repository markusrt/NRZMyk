using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NUnit.Framework;
using Tynamix.ObjectFiller;

namespace NRZMyk.Services.Tests.Data
{
    public class SentinelEntryRepositoryTests
    {
        private ApplicationDbContext _dbContext;

        private readonly Filler<SentinelEntry> _filler = new();
        
        [SetUp]
        public void Setup()
        {
            var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestCatalog")
                .Options;
            _dbContext = new ApplicationDbContext(dbOptions);

            _filler.Setup().OnProperty(e => e.PredecessorEntry).Use((SentinelEntry)null);
            _filler.Setup().OnProperty(e => e.PredecessorEntryId).Use((int?)null);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public void WhenNoEntriesExist_EntryNumberIsAssignedToOneAndYearToCurrent()
        {
            var entry = _filler.Create();
            var sut = CreateSut();

            sut.AssignNextEntryNumber(entry);
                
            entry.Year.Should().Be(DateTime.Now.Year);
            entry.YearlySequentialEntryNumber.Should().Be(1);
        }

        [Test]
        public async Task WhenOtherEntriesExist_EntryNumberIsAssignedCorrectly()
        {
            var entry = new SentinelEntry();
            var sut = CreateSut();
            foreach (var existingEntry in _filler.Create(10))
            {
                sut.AssignNextCryoBoxNumber(existingEntry);
                sut.AssignNextEntryNumber(existingEntry);
                await sut.AddAsync(existingEntry).ConfigureAwait(true);
            }

            sut.AssignNextEntryNumber(entry);
                
            entry.Year.Should().Be(DateTime.Now.Year);
            entry.YearlySequentialEntryNumber.Should().Be(11);
            entry.LaboratoryNumber.Should().Be($"SN-{DateTime.Now.Year}-0011");
        }

        [Test]
        public void WhenNoEntriesExist_CryoBoxAndSlotAreOne()
        {
            var entry = new SentinelEntry();
            var sut = CreateSut();

            sut.AssignNextCryoBoxNumber(entry);
                
            entry.CryoBoxNumber.Should().Be(1);
            entry.CryoBoxSlot.Should().Be(1);
        }

        [Test]
        public async Task WhenNoEntriesExist_CryoBoxAndSlotAreIncrementedAsExpectedDueToDefaultOptions()
        {
            var entry = new SentinelEntry();
            var sut = CreateSut(Options.Create(new ApplicationSettings()));
            foreach (var existingEntry in _filler.Create(100))
            {
                sut.AssignNextCryoBoxNumber(existingEntry);
                sut.AssignNextEntryNumber(existingEntry);
                await sut.AddAsync(existingEntry).ConfigureAwait(true);
            }

            sut.AssignNextCryoBoxNumber(entry);
                
            entry.CryoBoxNumber.Should().Be(2);
            entry.CryoBoxSlot.Should().Be(1);
            entry.CryoBox.Should().Be("SN-0002");
        }

        [Test]
        public async Task WhenNoOnlyOldYearEntryExists_NoNewCryoBoxIsStarted()
        {
            var entry = _filler.Create();
            var entryTwoYearsAgo = _filler.Create();
            entryTwoYearsAgo.CryoBoxSlot = 11;
            entryTwoYearsAgo.CryoBoxNumber = 40;
            entryTwoYearsAgo.Year = DateTime.Now.Year - 2;
            var sut = CreateSut(Options.Create(new ApplicationSettings()));
            await sut.AddAsync(entryTwoYearsAgo).ConfigureAwait(true);

            sut.AssignNextCryoBoxNumber(entry);
                
            entry.CryoBoxNumber.Should().Be(40);
            entry.CryoBoxSlot.Should().Be(12);
            entry.CryoBox.Should().Be("SN-0040");
        }

        [Test]
        public async Task WhenNoEntriesExist_CryoBoxAndSlotAreIncrementedAsExpectedDueToSpecificOptions()
        {
            var entry = new SentinelEntry();
            var sut = CreateSut(Options.Create(new ApplicationSettings {Application = new Application {CryoBoxSize = 7}}));
            foreach (var existingEntry in _filler.Create(30))
            {
                sut.AssignNextCryoBoxNumber(existingEntry);
                sut.AssignNextEntryNumber(existingEntry);
                await sut.AddAsync(existingEntry).ConfigureAwait(true);
            }

            sut.AssignNextCryoBoxNumber(entry);
                
            entry.CryoBoxNumber.Should().Be(5);
            entry.CryoBoxSlot.Should().Be(3);
        }

        private SentinelEntryRepository CreateSut(IOptions<ApplicationSettings> options = null)
        {
            return new SentinelEntryRepository(options, _dbContext);
        }
    }
}