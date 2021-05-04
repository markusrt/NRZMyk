using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.SuperUser))]
    public class ListByOrganization : BaseAsyncEndpoint<int, List<SentinelEntry>>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;

        public ListByOrganization(IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
        }

        [HttpGet("api/sentinel-entries/organization/{organizationId}")]
        [SwaggerOperation(
            Summary = "List Sentinel entries by organization",
            OperationId = "sentinel-entries.ListByOrganization",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<List<SentinelEntry>>> HandleAsync([FromRoute] int organizationId)
        {
            var items = await _sentinelEntryRepository.ListAsync(
                new SentinelEntryFilterSpecification($"{organizationId}"));
            return Ok(items);
        }
    }
}
