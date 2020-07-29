using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Controllers.CatalogItems;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize]
    public class Create : BaseAsyncEndpoint<CreateSentinelEntryRequest, SentinelEntry>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;

        public Create(IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
        }

        [HttpPost("api/sentinel-entries")]
        [SwaggerOperation(
            Summary = "Creates a new Sentinel Entry",
            OperationId = "catalog-entries.create",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<SentinelEntry>> HandleAsync(CreateSentinelEntryRequest request)
        {
            var newItem = new SentinelEntry
            {
                SamplingDate = request.SamplingDate,
                SenderLaboratoryNumber = request.SenderLaboratoryNumber,
                Material = request.Material,
                ResidentialTreatment = request.ResidentialTreatment,
                IdentifiedSpecies = request.IdentifiedSpecies,
                SpeciesTestingMethod = request.SpeciesTestingMethod,
                AgeGroup = request.AgeGroup,
                Remark = request.Remark
            };

            newItem = await _sentinelEntryRepository.AddAsync(newItem);

            return newItem;
        }
    }
}
