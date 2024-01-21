using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NRZMyk.Server.Authorization;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using NRZMyk.Services.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.User), Policy = Policies.AssignedToOrganization)]
    public class OtherLaboratoryNumbers : EndpointBaseAsync.WithoutRequest.WithActionResult<List<string>>
    {
        private readonly ISentinelEntryRepository _sentinelEntryRepository;

        public OtherLaboratoryNumbers(ISentinelEntryRepository sentinelEntryRepository)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
        }

        [HttpGet("api/sentinel-entries/other/laboratory-numbers")]
        [SwaggerOperation(
            Summary = "List all used other laboratory numbers)",
            OperationId = "sentinel-entries.OtherLaboratoryNumbers",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<List<string>>> HandleAsync(CancellationToken cancellationToken = new())
        {
            var organizationId = User.Claims.OrganizationId();
            var entriesForOrganization = await _sentinelEntryRepository.ListAsync(new SentinelEntryFilterSpecification(organizationId)).ConfigureAwait(false);
            var otherLaboratoryNumbers = entriesForOrganization.Select(e => e.LaboratoryNumber).Distinct().ToList();
            otherLaboratoryNumbers.Sort();
            return Ok(otherLaboratoryNumbers);
        }
    }
}
