using System;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize]
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
            var response = new ListPagedSentinelEntryResponse();

            var totalItems = await _sentinelEntryRepository.CountAsync(new SentinelEntryFilterSpecification());

            var pagedSpec = new SentinelEntryFilterPaginatedSpecification(
                request.PageIndex * request.PageSize,
                request.PageSize);

            var items = await _sentinelEntryRepository.ListAsync(pagedSpec);

            response.SentinelEntries.AddRange(items);
            response.PageCount = int.Parse(Math.Ceiling((decimal)totalItems / request.PageSize).ToString());

            return Ok(response);
        }
    }
}
