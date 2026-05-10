using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.SuperUser))]
    [Route("api/sentinel-entries/cryo-remark-update")]
    public class CryoRemarkUpdate : EndpointBaseAsync.WithRequest<CryoRemarkUpdateRequest>.WithActionResult<SentinelEntry>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;

        public CryoRemarkUpdate(IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Updates the cryo remark of a Sentinel Entry without changing storage status",
            OperationId = "sentinel-entries.cryo-remark-update",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<SentinelEntry>> HandleAsync(CryoRemarkUpdateRequest request, CancellationToken cancellationToken = new())
        {
            var existingItem = await _sentinelEntryRepository.FirstOrDefaultAsync(
                new SentinelEntryIncludingTestsSpecification(request.Id));
            
            if (existingItem == null)
            {
                return NotFound();
            }

            // Only update the CryoRemark field, leaving other fields untouched
            existingItem.CryoRemark = request.CryoRemark;
            
            await _sentinelEntryRepository.UpdateAsync(existingItem).ConfigureAwait(false);
            
            return Ok(existingItem);
        }
    }
}