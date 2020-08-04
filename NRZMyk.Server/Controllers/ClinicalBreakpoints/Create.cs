using System;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.ClinicalBreakpoints
{
    public class Create : BaseAsyncEndpoint<CreateClinicalBreakpointRequest, ClinicalBreakpoint>
    {
        private readonly IAsyncRepository<ClinicalBreakpoint> _clinicalBreakpointRepository;
        private readonly IMapper _mapper;

        public Create(IAsyncRepository<ClinicalBreakpoint> clinicalBreakpointRepository, IMapper mapper)
        {
            _clinicalBreakpointRepository = clinicalBreakpointRepository;
            _mapper = mapper;
        }

        [HttpPost("api/clinical-breakpoints")]
        [SwaggerOperation(
            Summary = "Creates a new Clinical Breakpoint",
            OperationId = "clinical-breakpoints.create",
            Tags = new[] { "SharedEndpoints" })
        ]
        public override async Task<ActionResult<ClinicalBreakpoint>> HandleAsync(CreateClinicalBreakpointRequest request)
        {
            var newEntry = _mapper.Map<ClinicalBreakpoint>(request);
            return await _clinicalBreakpointRepository.AddAsync(newEntry);
        }
    }
}
