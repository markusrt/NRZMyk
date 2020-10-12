using System.Collections.Generic;
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
    [Authorize(Roles = nameof(Role.Admin))]
    public class ListUsers : BaseAsyncEndpoint<List<RemoteAccount>>
    {
        private readonly IAsyncRepository<RemoteAccount> _accountRepository;

        public ListUsers(IAsyncRepository<RemoteAccount> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet("api/users")]
        [SwaggerOperation(
            Summary = "List users",
            OperationId = "account.list",
            Tags = new[] { "AccountEndpoints" })
        ]
        public override async Task<ActionResult<List<RemoteAccount>>> HandleAsync()
        {
            var items = await _accountRepository.ListAllAsync();
            return Ok(items);
        }
    }
}
