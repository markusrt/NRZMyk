using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NRZMyk.Server.Invocables;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Server.Tests.Invocables;

public class SentinelEntryReminderEmailJobTests
{
   private List<Organization> _organizations;

   private ApplicationDbContext _context;

   private IEmailNotificationService _emailNotificationService;

   [SetUp]
   public void SetUp()
   {
       _emailNotificationService = Substitute.For<IEmailNotificationService>();
       _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
       _organizations = new List<Organization>
       {
           new() { Id = 1, Name = "Org One", DispatchMonth = MonthToDispatch.January, Members = new List<RemoteAccount>() },
           new() { Id = 2, Name = "Org Two", DispatchMonth = MonthToDispatch.February, Members = new List<RemoteAccount>() }
       };
   }

   [TearDown]
   public void TearDown()
   {
       _context = null;
   }

   private void InitTestData()
   {
       foreach (var organization in _organizations)
       {
           _context.Organizations.Add(organization);
       }

       _context.RemoteAccounts.Add(new RemoteAccount { OrganizationId = 1, Email = "user1@org1.de"});
       _context.RemoteAccounts.Add(new RemoteAccount { OrganizationId = 2, Email = "user1@org2.de" });
       _context.RemoteAccounts.Add(new RemoteAccount { OrganizationId = 2, Email = "user2@org2.de" });
       _context.SaveChanges();

       _context.ChangeTracker.Entries()
           .Where(e => e.Entity != null)
           .ToList()
           .ForEach(e => e.State = EntityState.Detached);
   }

   [Test]
   public async Task CheckDataAndSendReminders_JobIsDisabled_DoesNotSendReminder()
   {
       _organizations[0].DispatchMonth = (MonthToDispatch)DateTime.Now.Month;
       _organizations[1].DispatchMonth = (MonthToDispatch)DateTime.Now.Month;
       InitTestData();
       var organization1 = (await _context.Organizations.FindAsync(1))!;
       var organization2 = (await _context.Organizations.FindAsync(2))!;

       var sut = CreateSut(JobSetting.Disabled);

       await sut.Invoke();
       

       await _emailNotificationService.DidNotReceive().RemindOrganizationOnDispatchMonth(organization1);
       (await _context.Organizations.FindAsync(1))!.LastReminderSent.Should().BeNull();
       await _emailNotificationService.DidNotReceive().RemindOrganizationOnDispatchMonth(organization2);
       (await _context.Organizations.FindAsync(2))!.LastReminderSent.Should().BeNull();
   }

   [Test]
   public async Task CheckDataAndSendReminders_LastReminderSentIsEmptyAndSendingIsDueThisMonth_SendsReminder()
   {
       _organizations[0].DispatchMonth = (MonthToDispatch)DateTime.Now.Month;
       _organizations[1].DispatchMonth = (MonthToDispatch)DateTime.Now.AddMonths(1).Month;
       InitTestData();
       var organization1 = (await _context.Organizations.FindAsync(1))!;
       var organization2 = (await _context.Organizations.FindAsync(2))!;

       var sut = CreateSut();

       await sut.Invoke();

       await _emailNotificationService.Received(1).RemindOrganizationOnDispatchMonth(organization1);
       (await _context.Organizations.FindAsync(1))!.LastReminderSent.Should().Be(DateTime.Today);

       await _emailNotificationService.DidNotReceive().RemindOrganizationOnDispatchMonth(organization2);
   }

   [Test]
   public async Task CheckDataAndSendReminders_OrganizationHasMembers_IncludesMembers()
   {
       _organizations[0].DispatchMonth = (MonthToDispatch)DateTime.Now.Month;
       _organizations[1].DispatchMonth = (MonthToDispatch)DateTime.Now.AddMonths(1).Month;
       InitTestData();
       Organization organization1 = null;
       await _emailNotificationService.RemindOrganizationOnDispatchMonth(Arg.Do<Organization>(arg => organization1 = arg));
       
       var sut = CreateSut();

       await sut.Invoke();

       organization1.Members.Should().HaveCount(1);
   }

   [Test]
   public async Task CheckDataAndSendReminders_LastReminderSentIsThisMonthAndSendingIsDueThisMonth_DoesNotSendsReminder()
   {
       _organizations[0].DispatchMonth = (MonthToDispatch)DateTime.Now.Month;
       _organizations[1].DispatchMonth = (MonthToDispatch)DateTime.Now.Month;
       _organizations[1].LastReminderSent = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 20);
       InitTestData();
       var organization1 = (await _context.Organizations.FindAsync(1))!;
       var organization2 = (await _context.Organizations.FindAsync(2))!;

       var sut = CreateSut();

       await sut.Invoke();

       await _emailNotificationService.Received(1).RemindOrganizationOnDispatchMonth(organization1);
       (await _context.Organizations.FindAsync(1))!.LastReminderSent.Should().Be(DateTime.Today);

       await _emailNotificationService.DidNotReceive().RemindOrganizationOnDispatchMonth(organization2);
   }

   [Test]
   public async Task CheckDataAndSendReminders_LastReminderSentIsLastYear_SendsReminder()
   {
       _organizations[0].DispatchMonth = (MonthToDispatch)DateTime.Now.AddMonths(1).Month;
       _organizations[1].DispatchMonth = (MonthToDispatch)DateTime.Now.Month;
       _organizations[1].LastReminderSent = new DateTime(DateTime.Now.Year-1, DateTime.Now.Month, 20);
       InitTestData();
       var organization1 = (await _context.Organizations.FindAsync(1))!;
       var organization2 = (await _context.Organizations.FindAsync(2))!;

       var sut = CreateSut();

       await sut.Invoke();

       await _emailNotificationService.DidNotReceive().RemindOrganizationOnDispatchMonth(organization1);
       
       await _emailNotificationService.Received(1).RemindOrganizationOnDispatchMonth(organization2);
       (await _context.Organizations.FindAsync(2))?.LastReminderSent.Should().Be(DateTime.Today);
   }

   private SentinelEntryReminderEmailJob CreateSut(JobSetting setting = JobSetting.Enabled)
   {
       var options = Options.Create(new ApplicationSettings { Application = new Application {SentinelReminderJob = setting}});
       return new SentinelEntryReminderEmailJob(
           NullLogger<SentinelEntryReminderEmailJob>.Instance, options, new EfRepository<Organization>(_context),
           new EfRepository<SentinelEntry>(_context), _emailNotificationService);
   }
}