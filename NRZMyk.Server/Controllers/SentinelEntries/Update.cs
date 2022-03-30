using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Authorization;
using NRZMyk.Server.Utils;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NRZMyk.Services.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.User), Policy = Policies.AssignedToOrganization)]
    public class Update : BaseAsyncEndpoint<SentinelEntryRequest, SentinelEntry>
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
        public override async Task<ActionResult<SentinelEntry>> HandleAsync(SentinelEntryRequest request)
        {
            var organizationId = User.Claims.OrganizationId();
            var existingItem = (await _sentinelEntryRepository.FirstOrDefaultAsync(
                new SentinelEntryIncludingTestsSpecification(request.Id)));

            if (existingItem.IsNullOrProtected(User, Role.SuperUser))
            {
                return NotFound();
            }

            if (existingItem.CryoDate.HasValue)
            {
                return Forbid();
            }

            //TODO make update a bit more intelligent, i.e. check if ids are same instead of deleting all...
            foreach (var sensitivityTest in existingItem.AntimicrobialSensitivityTests.ToList())
            {
                await _sensitivityTestRepository.DeleteAsync(sensitivityTest).ConfigureAwait(false);
            }

            existingItem.PredecessorEntryId = null;
            existingItem.PredecessorEntry = null;

            var error = await Utils.ResolvePredecessor(request, existingItem, _sentinelEntryRepository, organizationId, ModelState).ConfigureAwait(false);
            if (error)
            {
                return new BadRequestObjectResult(ModelState);
            }
            
            _mapper.Map(request, existingItem);
            await _sentinelEntryRepository.UpdateAsync(existingItem).ConfigureAwait(false);
            
            return Ok(existingItem);
        }
    }
}
