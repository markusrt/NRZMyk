using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.Account
{
    [Authorize(Roles = nameof(Role.SuperUser))]
    public class List : EndpointBaseAsync.WithoutRequest.WithActionResult<List<RemoteAccount>>
    {
        private readonly IAsyncRepository<Organization> _organizationRepository;

        public List(IAsyncRepository<Organization> organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        [HttpGet("api/organizations")]
        [SwaggerOperation(
            Summary = "List organizations",
            OperationId = "organization.list",
            Tags = new[] { "AccountEndpoints" })
        ]
        public override async Task<ActionResult<List<RemoteAccount>>> HandleAsync(CancellationToken cancellationToken = new())
        {
            var items = await _organizationRepository.ListAllAsync().ConfigureAwait(false);
            return Ok(items);
        }
    }
}
