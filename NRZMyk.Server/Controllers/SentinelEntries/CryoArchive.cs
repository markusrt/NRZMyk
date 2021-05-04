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
    public class CryoArchive : BaseAsyncEndpoint<CryoArchiveRequest, SentinelEntry>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;
        private readonly IMapper _mapper;

        public CryoArchive(IAsyncRepository<SentinelEntry> sentinelEntryRepository, 
            IMapper mapper)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _mapper = mapper;
        }

        [HttpPut("api/sentinel-entries/cryo-archive")]
        [SwaggerOperation(
            Summary = "Store or releases a Sentinel Entry from cryo storage",
            OperationId = "sentinel-entries.cryo-archive",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<SentinelEntry>> HandleAsync(CryoArchiveRequest request)
        {
            var existingItem = await _sentinelEntryRepository.FirstOrDefaultAsync(
                new SentinelEntryIncludingTestsSpecification(request.Id));
            
            _mapper.Map(request, existingItem);
            await _sentinelEntryRepository.UpdateAsync(existingItem);
            
            return Ok(existingItem);
        }
    }
}
