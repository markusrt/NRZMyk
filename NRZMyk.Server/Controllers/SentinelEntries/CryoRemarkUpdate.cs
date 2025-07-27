using System.Threading;
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
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.SuperUser))]
    public class CryoRemarkUpdate : EndpointBaseAsync.WithRequest<CryoRemarkUpdateRequest>.WithActionResult<SentinelEntry>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;
        private readonly IMapper _mapper;

        public CryoRemarkUpdate(IAsyncRepository<SentinelEntry> sentinelEntryRepository, 
            IMapper mapper)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _mapper = mapper;
        }

        [HttpPut("api/sentinel-entries/cryo-remark-update")]
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