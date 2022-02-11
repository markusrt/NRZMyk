using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data;
using NRZMyk.Services.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.User))]
    public class OtherSpecies : BaseAsyncEndpoint<List<string>>
    {
        private readonly ISentinelEntryRepository _sentinelEntryRepository;
        private readonly List<string> _configuredOtherSpecies;

        public OtherSpecies(ISentinelEntryRepository sentinelEntryRepository, IOptions<ApplicationSettings> config)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _configuredOtherSpecies = config?.Value?.Application?.OtherSpecies ?? new List<string>();
        }

        [HttpGet("api/sentinel-entries/other/species")]
        [SwaggerOperation(
            Summary = "List all used other species)",
            OperationId = "sentinel-entries.OtherSpecies",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<List<string>>> HandleAsync()
        {
            var otherSpecies =  await _sentinelEntryRepository.Other(s => s.OtherIdentifiedSpecies).ConfigureAwait(false);
            return Ok(otherSpecies.Union(_configuredOtherSpecies).Distinct().OrderBy(s=>s));
        }
    }
}
