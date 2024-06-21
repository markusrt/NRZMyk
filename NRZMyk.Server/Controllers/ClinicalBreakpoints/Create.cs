using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.ClinicalBreakpoints
{
    [Authorize(Roles=nameof(Role.Admin))]
    public class Create : EndpointBaseAsync.WithRequest<CreateClinicalBreakpointRequest>.WithActionResult<ClinicalBreakpoint>
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
        public override async Task<ActionResult<ClinicalBreakpoint>> HandleAsync(CreateClinicalBreakpointRequest request, CancellationToken cancellationToken = new())
        {
            var newEntry = _mapper.Map<ClinicalBreakpoint>(request);
            return await _clinicalBreakpointRepository.AddAsync(newEntry).ConfigureAwait(false);
        }
    }
}
