using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Authorization;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using NRZMyk.Services.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.User), Policy = Policies.AssignedToOrganization)]
    public class ListPaged : EndpointBaseAsync.WithRequest<ListPagedSentinelEntryRequest>.WithActionResult<ListPagedSentinelEntryResponse>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;

        public ListPaged(IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
        }

        [HttpGet("api/sentinel-entries")]
        [SwaggerOperation(
            Summary = "List Sentinel entries (paged)",
            OperationId = "sentinel-entries.ListPaged",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<ListPagedSentinelEntryResponse>> HandleAsync([FromQuery]ListPagedSentinelEntryRequest request, CancellationToken cancellationToken = new())
        {
            var organizationId = User.Claims.OrganizationId();
            var response = new ListPagedSentinelEntryResponse();

            var totalItems = await _sentinelEntryRepository.CountAsync(new SentinelEntryFilterSpecification(organizationId)).ConfigureAwait(false);

            var pagedSpec = new SentinelEntryFilterPaginatedSpecification(
                request.PageIndex * request.PageSize,
                request.PageSize, organizationId);

            var items = await _sentinelEntryRepository.ListAsync(pagedSpec).ConfigureAwait(false);

            response.SentinelEntries.AddRange(items);
            response.PageCount = int.Parse(Math.Ceiling((decimal)totalItems / request.PageSize).ToString());

            return Ok(response);
        }
    }
}
