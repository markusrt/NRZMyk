﻿using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RemoteAccount> RemoteAccounts { get; set; }

        public DbSet<SentinelEntry> SentinelEntries { get; set; }

        public DbSet<ClinicalBreakpoint> ClinicalBreakpoints { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<SentinelEntry>()
                .HasIndex(p => new {p.CryoBoxNumber , p.CryoBoxSlot}).IsUnique();
            builder.Entity<SentinelEntry>()
                .HasIndex(p => new {p.Year , p.YearlySequentialEntryNumber}).IsUnique();

            builder.Entity<RemoteAccount>().Property(
                a => a.ObjectId).IsRequired();
            builder.Entity<RemoteAccount>().Property(
                a => a.Email).IsRequired();
            builder.Entity<RemoteAccount>()
                .HasIndex(a => a.ObjectId).IsUnique();
        }
    }
}
