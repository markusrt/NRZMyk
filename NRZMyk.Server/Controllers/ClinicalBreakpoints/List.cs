using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.ClinicalBreakpoints
{
    [Authorize]
    public class List : BaseAsyncEndpoint<ListClinicalBreakpointsRequest, List<ClinicalBreakpointReference>>
    {
        private readonly IAsyncRepository<ClinicalBreakpoint> _clinicalBreakpointRepository;
        private readonly IMapper _mapper;

        public List(IAsyncRepository<ClinicalBreakpoint> clinicalBreakpointRepository, IMapper mapper)
        {
            _clinicalBreakpointRepository = clinicalBreakpointRepository;
            _mapper = mapper;
        }

        [HttpGet("api/clinical-breakpoints")]
        [SwaggerOperation(
            Summary = "List Clinical Breakpoints",
            OperationId = "clinical-breakpoints.list",
            Tags = new[] { "SharedEndpoints" })
        ]
        public override async Task<ActionResult<List<ClinicalBreakpointReference>>> HandleAsync([FromQuery]ListClinicalBreakpointsRequest request)
        {
            var filter = new ClinicalBreakpointFilterSpecification(request.Species);
            var items = await _clinicalBreakpointRepository.ListAsync(filter);
            var referenceItems = _mapper.Map<List<ClinicalBreakpointReference>>(items);
            return Ok(referenceItems);
        }
    }
}
