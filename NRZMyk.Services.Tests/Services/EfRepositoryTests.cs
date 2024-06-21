using System;
using System.Threading.Tasks;
using Ardalis.Specification;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Services;

public class EfRepositoryTests
{
    private ApplicationDbContext _context;

    public EfRepositoryTests()
    {
    }

    [SetUp]
    public void SetUp()
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("ApplicationDbContext")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new ApplicationDbContext(contextOptions);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _context.AddRange(
            new Organization { Id = 1, Name = "Org1" },
            new Organization { Id = 2, Name = "Org2" }
        );

        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public void Ctor_DoesNotThrow()
    {
        Action createSut = () => CreateSut();

        createSut.Should().NotThrow();
    }

    [Test]
    public async Task WhenGetById_ReturnsEntity()
    {
        var organization = await CreateSut().GetByIdAsync(1).ConfigureAwait(true);

        organization.Name.Should().Be("Org1");
    }

    [Test]
    public async Task WhenListAll_ReturnsAll()
    {
        var organizations = await CreateSut().ListAllAsync().ConfigureAwait(true);

        organizations.Should().HaveCount(2);
    }

    [Test]
    public async Task WhenListWithSpec_ReturnsFiltered()
    {
        var organizations = await CreateSut().ListAsync(new Org1Spec()).ConfigureAwait(true);

        organizations.Should().HaveCount(1);
    }

    [Test]
    public async Task WhenCountWithSpec_ReturnsFiltered()
    {
        var organizations = await CreateSut().CountAsync(new Org1Spec()).ConfigureAwait(true);

        organizations.Should().Be(1);
    }

    [Test]
    public async Task WhenAdd_InsertsOneMore()
    {
        await CreateSut().AddAsync(new Organization() {Id = 3, Name = "Org3"}).ConfigureAwait(true);

        var organizations = await CreateSut().ListAllAsync().ConfigureAwait(true);
        organizations.Should().HaveCount(3);
    }

    [Test]
    public async Task WhenFirstUpdateFirstOrDefault_BehavesAsExpected()
    {
        var org1 = await CreateSut().FirstAsync(new Org1Spec()).ConfigureAwait(true);
        org1.Name = "Org3";

        await CreateSut().UpdateAsync(org1).ConfigureAwait(true);

        var organizations = await CreateSut().FirstOrDefaultAsync(new Org1Spec()).ConfigureAwait(true);
        organizations.Should().BeNull();
    }

    [Test]
    public async Task WhenDelete_RemovesEntity()
    {
        var org1 = await CreateSut().FirstAsync(new Org1Spec()).ConfigureAwait(true);

        await CreateSut().DeleteAsync(org1).ConfigureAwait(true);

        var organizations = await CreateSut().FirstOrDefaultAsync(new Org1Spec()).ConfigureAwait(true);
        organizations.Should().BeNull();
    }

    private EfRepository<Organization> CreateSut()
    {
        return new EfRepository<Organization>(_context);
    }

    private sealed class Org1Spec : Specification<Organization>
    {
        public Org1Spec()
        {
            Query.Where(o => o.Name == "Org1");
        }
    }
}