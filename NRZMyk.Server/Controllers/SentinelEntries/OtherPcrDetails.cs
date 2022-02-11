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
    public class OtherPcrDetails : BaseAsyncEndpoint<List<string>>
    {
        private readonly ISentinelEntryRepository _sentinelEntryRepository;

        public OtherPcrDetails(ISentinelEntryRepository sentinelEntryRepository, IOptions<ApplicationSettings> config)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
        }

        [HttpGet("api/sentinel-entries/other/pcr-details")]
        [SwaggerOperation(
            Summary = "List all used other species)",
            OperationId = "sentinel-entries.OtherPcrDetails",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<List<string>>> HandleAsync()
        {
            var otherPcrDetails =  await _sentinelEntryRepository.Other(s => s.PcrDetails).ConfigureAwait(false);
            return Ok(otherPcrDetails);
        }
    }
}
