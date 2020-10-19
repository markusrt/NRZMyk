using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.User))]
    public class OtherMaterials : BaseAsyncEndpoint<List<string>>
    {
        private readonly ISentinelEntryRepository _sentinelEntryRepository;

        public OtherMaterials(ISentinelEntryRepository sentinelEntryRepository)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
        }

        [HttpGet("api/sentinel-entries/other/materials")]
        [SwaggerOperation(
            Summary = "List all used other materials)",
            OperationId = "sentinel-entries.OtherMaterials",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<List<string>>> HandleAsync()
        {
            var otherMaterials =  await _sentinelEntryRepository.Other(s => s.OtherMaterial);
            return Ok(otherMaterials);
        }
    }
}
