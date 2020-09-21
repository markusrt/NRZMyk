using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    public class GetById : BaseAsyncEndpoint<int, SentinelEntry>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;

        public GetById(IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
        }

        [HttpGet("api/sentinel-entries/{sentinelEntryId}")]
        [SwaggerOperation(
            Summary = "Get a Sentinel Entry by Id",
            OperationId = "sentinel-entries.GetById",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<SentinelEntry>> HandleAsync([FromRoute] int sentinelEntryId)
        {
            var sentinelEntry = (await _sentinelEntryRepository.FirstOrDefaultAsync(
                new SentinelEntryIncludingTestsSpecification(sentinelEntryId)));
            if (sentinelEntry is null)
            {
                return NotFound();
            }

            return Ok(sentinelEntry);
        }
    }
}
