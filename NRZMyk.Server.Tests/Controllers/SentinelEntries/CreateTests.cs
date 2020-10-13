using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class CreateTests
    {
        [Test]
        public async Task WhenCreatedWithoutOrganization_AccessDenied()
        {
            var sut = CreateSut(out var repository, out var mapper);
            var createSentinelEntryRequest = new SentinelEntryRequest();
            var sentinelEntry = new SentinelEntry {Id = 123};

            var action = await sut.HandleAsync(createSentinelEntryRequest);
            
            action.Result.Should().BeOfType<ForbidResult>();
            await repository.Received(0).AddAsync(sentinelEntry);
        }

        [Test]
        public async Task WhenCreated_MapsAndStoresToRepository()
        {
            var sut = CreateSut(out var repository, out var mapper, "12");
            var createSentinelEntryRequest = new SentinelEntryRequest();
            var sentinelEntry = new SentinelEntry {Id = 123};
            mapper.Map<SentinelEntry>(createSentinelEntryRequest).Returns(sentinelEntry);
            repository.AddAsync(sentinelEntry).Returns(sentinelEntry);

            var action = await sut.HandleAsync(createSentinelEntryRequest);

            await repository.Received(1).AddAsync(sentinelEntry);
            sentinelEntry.ProtectKey.Should().Be("12");
            var createdResult = action.Result.Should().BeOfType<CreatedResult>().Subject;
            createdResult.Value.Should().Be(sentinelEntry);
            createdResult.Location.Should().Be("http://localhost/api/sentinel-entries/123");
        }

        private static Create CreateSut(out ISentinelEntryRepository sentinelEntryRepository, out IMapper map, string organizationId = null)
        {
            sentinelEntryRepository = Substitute.For<ISentinelEntryRepository>();
            map = Substitute.For<IMapper>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("localhost");
            httpContext.Request.Scheme = "http";
            httpContext.Request.Path = new PathString("/api/sentinel-entries");
            var identity = new ClaimsIdentity();
            if (organizationId != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Organization, organizationId));
            }
            httpContext.User = new ClaimsPrincipal(identity);
            return new Create(sentinelEntryRepository, map)
            {
                ControllerContext = new ControllerContext {
                    HttpContext = httpContext,
                }
            };
        }
    }
}