using System;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize]
    public class Create : BaseAsyncEndpoint<CreateSentinelEntryRequest, SentinelEntry>
    {
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;
        private readonly IMapper _mapper;

        public Create(IAsyncRepository<SentinelEntry> sentinelEntryRepository, IMapper mapper)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _mapper = mapper;
        }

        [HttpPost("api/sentinel-entries")]
        [SwaggerOperation(
            Summary = "Creates a new Sentinel Entry",
            OperationId = "catalog-entries.create",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<SentinelEntry>> HandleAsync(CreateSentinelEntryRequest request)
        {
            var newEntry = _mapper.Map<SentinelEntry>(request);
            var storedEntry = await _sentinelEntryRepository.AddAsync(newEntry);
            return Created(new Uri($"{Request.GetUri()}/{storedEntry.Id}"), storedEntry);
        }
    }
}
