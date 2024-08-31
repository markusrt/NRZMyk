using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Server.Utils;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.Account
{
    [Authorize(Roles = Roles.RegularUsers)]
    public class ListOrganizations : EndpointBaseAsync.WithoutRequest.WithActionResult<List<Organization>>
    {
        private readonly IAsyncRepository<Organization> _organizationRepository;
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;

        public ListOrganizations(IAsyncRepository<Organization> organizationRepository, IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            _organizationRepository = organizationRepository;
            _sentinelEntryRepository = sentinelEntryRepository;
        }

        [HttpGet("api/organizations")]
        [SwaggerOperation(
            Summary = "List organizations",
            OperationId = "organization.list",
            Tags = new[] { "AccountEndpoints" })
        ]
        public override async Task<ActionResult<List<Organization>>> HandleAsync(CancellationToken cancellationToken = new())
        {
            var organizations = await _organizationRepository.ListAllWithDatesAsync(_sentinelEntryRepository);
            return Ok(organizations);
        }

    }
}
