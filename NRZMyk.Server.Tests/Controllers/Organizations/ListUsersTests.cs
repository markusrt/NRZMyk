﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Server.Controllers.Account;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;
using Tynamix.ObjectFiller;

namespace NRZMyk.Server.Tests.Controllers.Organizations
{
    public class ListOrganizationsTests
    {
        private readonly List<Organization> _organizations = new()
        {
            new Organization { Id = 1, DispatchMonth = MonthToDispatch.January, Email = "one@org.de", Members = new List<RemoteAccount>() },
            new Organization { Id = 2, DispatchMonth = MonthToDispatch.February, Email = "two@org.de", Members = new List<RemoteAccount>() }
        };

        [Test]
        public async Task WhenQueryingOrganizationsWithExistingEntries_AddsLatestCryoAndSamplingDates()
        {
            var sut = CreateSut(out _, out var sentinelEntryRepository);
            var samplingDate = new DateTime(2010, 10, 10);
            sentinelEntryRepository.FirstOrDefaultAsync(Arg.Any<SentinelEntryBySamplingDateSpecification>()).Returns(
                new SentinelEntry { SamplingDate = samplingDate });
            var cryoDate = new DateTime(2010, 7, 7);
            sentinelEntryRepository.FirstOrDefaultAsync(Arg.Any<SentinelEntryByCryoDateSpecification>()).Returns(
                new SentinelEntry { CryoDate = cryoDate });
            
            var expectedResult = new List<Organization>(_organizations);
            foreach (var organization in expectedResult)
            {
                organization.LatestCryoDate = cryoDate;
                organization.LatestSamplingDate = samplingDate;
            }
            
            var action = await sut.HandleAsync().ConfigureAwait(true);

            action.Result.Should().BeOfType<OkObjectResult>();
            var organizations = action.Result.As<OkObjectResult>().Value;
            organizations.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task WhenQueryingOrganizationsWithNoEntries_LeavesCryoAndSamplingDateEmpty()
        {
            var sut = CreateSut(out _, out var sentinelEntryRepository);
            sentinelEntryRepository.FirstOrDefaultAsync(Arg.Any<SentinelEntryBySamplingDateSpecification>()).Returns((SentinelEntry)null);
            sentinelEntryRepository.FirstOrDefaultAsync(Arg.Any<SentinelEntryByCryoDateSpecification>()).Returns((SentinelEntry)null);
            
            var expectedResult = new List<Organization>(_organizations);
            
            var action = await sut.HandleAsync().ConfigureAwait(true);

            action.Result.Should().BeOfType<OkObjectResult>();
            var organizations = action.Result.As<OkObjectResult>().Value;
            organizations.Should().BeEquivalentTo(expectedResult);
        }



        private ListOrganizations CreateSut(out IAsyncRepository<Organization> organizationRepository, out IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            organizationRepository = Substitute.For<IAsyncRepository<Organization>>();
            organizationRepository.ListAllAsync()
                .Returns(Task.FromResult((IReadOnlyList<Organization>)_organizations));

            sentinelEntryRepository = Substitute.For<IAsyncRepository<SentinelEntry>>();
            
            return new ListOrganizations(organizationRepository, sentinelEntryRepository)
            {
                ControllerContext = new MockControllerContext()
            };
        }
    }
}