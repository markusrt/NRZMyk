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
            // Determine the organization ID to filter by
            string protectKey = null;
            if (request.OrganizationId.HasValue && request.OrganizationId.Value > 0)
            {
                // Super user can specify organization
                if (User.IsInRole(nameof(Role.SuperUser)))
                {
                    protectKey = request.OrganizationId.Value.ToString();
                }
                else
                {
                    // Regular users can only access their own organization
                    var userOrgKey = User.Claims.OrganizationId();
                    if (request.OrganizationId.Value.ToString() == userOrgKey)
                    {
                        protectKey = userOrgKey;
                    }
                    else
                    {
                        return Forbid();
                    }
                }
            }
            else
            {
                // Default to user's organization
                protectKey = User.Claims.OrganizationId();
            }

            var response = new ListPagedSentinelEntryResponse();

            var countSpec = new SentinelEntrySearchFilterSpecification(protectKey, request.SearchTerm);
            var totalItems = await _sentinelEntryRepository.CountAsync(countSpec).ConfigureAwait(false);

            var pagedSpec = new SentinelEntrySearchPaginatedSpecification(
                request.PageIndex * request.PageSize,
                request.PageSize, 
                protectKey, 
                request.SearchTerm);

            var items = await _sentinelEntryRepository.ListAsync(pagedSpec).ConfigureAwait(false);

            response.SentinelEntries.AddRange(items);
            response.PageCount = int.Parse(Math.Ceiling((decimal)totalItems / request.PageSize).ToString());

            return Ok(response);
        }
    }
}
