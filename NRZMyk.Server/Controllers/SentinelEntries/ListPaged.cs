using System;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using NRZMyk.Services.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.User))]
    public class ListPaged : BaseAsyncEndpoint<ListPagedSentinelEntryRequest, ListPagedSentinelEntryResponse>
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
        public override async Task<ActionResult<ListPagedSentinelEntryResponse>> HandleAsync([FromQuery]ListPagedSentinelEntryRequest request)
        {
            var organizationId = User.Claims.OrganizationId();
            if (string.IsNullOrEmpty(organizationId))
            {
                return Forbid();
            }

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
