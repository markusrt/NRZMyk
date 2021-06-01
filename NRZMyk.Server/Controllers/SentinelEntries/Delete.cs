using System.Linq;
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
    public class Delete : BaseAsyncEndpoint<int, int>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;
        private readonly IAsyncRepository<AntimicrobialSensitivityTest> _sensitivityTestRepository;

        public Delete(IAsyncRepository<SentinelEntry> sentinelEntryRepository,
            IAsyncRepository<AntimicrobialSensitivityTest> sensitivityTestRepository)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _sensitivityTestRepository = sensitivityTestRepository;
        }

        [HttpDelete("api/sentinel-entries/{sentinelEntryId}")]
        [SwaggerOperation(
            Summary = "Delete a Sentinel Entry by Id",
            OperationId = "sentinel-entries.DeleteById",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<int>> HandleAsync([FromRoute] int sentinelEntryId)
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

            if (sentinelEntry.CryoDate.HasValue)
            {
                return Forbid();
            }

            foreach (var sensitivityTest in sentinelEntry.AntimicrobialSensitivityTests.ToList())
            {
                //TODO isn't there a better way to cascade delete one-to-many?
                await _sensitivityTestRepository.DeleteAsync(sensitivityTest);
            }
            await _sentinelEntryRepository.DeleteAsync(sentinelEntry);
            
            return Ok(sentinelEntry.Id);
        }
    }
}
