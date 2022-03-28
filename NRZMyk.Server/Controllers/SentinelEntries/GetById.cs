using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NRZMyk.Services.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.User))]
    public class GetById : BaseAsyncEndpoint<int, SentinelEntryResponse>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;
        private readonly IMapper _mapper;

        public GetById(IAsyncRepository<SentinelEntry> sentinelEntryRepository, IMapper mapper)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _mapper = mapper;
        }

        [HttpGet("api/sentinel-entries/{sentinelEntryId}")]
        [SwaggerOperation(
            Summary = "Get a Sentinel Entry by Id",
            OperationId = "sentinel-entries.GetById",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<SentinelEntryResponse>> HandleAsync([FromRoute] int sentinelEntryId)
        {
            var organizationId = User.Claims.OrganizationId();
            if (string.IsNullOrEmpty(organizationId))
            {
                return Forbid();
            }
            var sentinelEntry = (await _sentinelEntryRepository.FirstOrDefaultAsync(
                new SentinelEntryIncludingTestsSpecification(sentinelEntryId)));
            if (sentinelEntry is null || sentinelEntry.ProtectKey != organizationId)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<SentinelEntryResponse>(sentinelEntry));
        }
    }
}
