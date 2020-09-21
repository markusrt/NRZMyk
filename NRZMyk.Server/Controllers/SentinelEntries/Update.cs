using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize]
    public class Update : BaseAsyncEndpoint<UpdateSentinelEntryRequest, SentinelEntry>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;
        private readonly IAsyncRepository<AntimicrobialSensitivityTest> _sensitivityTestRepository;
        private readonly IMapper _mapper;

        public Update(IAsyncRepository<SentinelEntry> sentinelEntryRepository, 
            IAsyncRepository<AntimicrobialSensitivityTest> sensitivityTestRepository, 
            IMapper mapper)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _sensitivityTestRepository = sensitivityTestRepository;
            _mapper = mapper;
        }

        [HttpPut("api/sentinel-entries")]
        [SwaggerOperation(
            Summary = "Updates a Sentinel Entry",
            OperationId = "sentinel-entries.update",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<SentinelEntry>> HandleAsync(UpdateSentinelEntryRequest request)
        {
            var existingItem = (await _sentinelEntryRepository.FirstOrDefaultAsync(
                new SentinelEntryIncludingTestsSpecification(request.Id)));
            if (existingItem == null)
            {
                return NotFound();
            }

            //TODO make update a bit more intelligent, i.e. check if ids are same instead of deleting all...
            foreach (var sensitivityTest in existingItem.AntimicrobialSensitivityTests.ToList())
            {
                await _sensitivityTestRepository.DeleteAsync(sensitivityTest);
            }
            
            _mapper.Map(request, existingItem);
            await _sentinelEntryRepository.UpdateAsync(existingItem);
            
            return Ok(existingItem);
        }
    }
}
