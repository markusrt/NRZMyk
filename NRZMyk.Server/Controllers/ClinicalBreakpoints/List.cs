﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.ClinicalBreakpoints
{
    [Authorize]
    public class List : EndpointBaseAsync.WithRequest<ListClinicalBreakpointsRequest>.WithActionResult<List<ClinicalBreakpoint>>
    {
        private readonly IAsyncRepository<ClinicalBreakpoint> _clinicalBreakpointRepository;

        public List(IAsyncRepository<ClinicalBreakpoint> clinicalBreakpointRepository)
        {
            _clinicalBreakpointRepository = clinicalBreakpointRepository;
        }

        [HttpGet("api/clinical-breakpoints")]
        [SwaggerOperation(
            Summary = "List Clinical Breakpoints",
            OperationId = "clinical-breakpoints.list",
            Tags = new[] { "SharedEndpoints" })
        ]
        public override async Task<ActionResult<List<ClinicalBreakpoint>>> HandleAsync([FromQuery]ListClinicalBreakpointsRequest request, CancellationToken cancellationToken = new())
        {
            var filter = new ClinicalBreakpointFilterSpecification(request.Species);
            var items = await _clinicalBreakpointRepository.ListAsync(filter).ConfigureAwait(false);
            return Ok(items);
        }
    }
}
