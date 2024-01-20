using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.Account
{
    [Authorize(Roles = nameof(Role.Admin))]
    public class AssignOrganization : EndpointBaseAsync.WithRequest<List<RemoteAccount>>.WithActionResult<int>
    {
        private readonly IAsyncRepository<RemoteAccount> _accountRepository;
        private readonly ILogger<AssignOrganization> _logger;

        public AssignOrganization(IAsyncRepository<RemoteAccount> accountRepository, ILogger<AssignOrganization> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        [HttpPost("api/users/assign-organization")]
        [SwaggerOperation(
            Summary = "Assign organization to account",
            OperationId = "account.assign-organization",
            Tags = new[] { "AccountEndpoints" })
        ]
        public override async Task<ActionResult<int>> HandleAsync(List<RemoteAccount> accountsToUpdate, CancellationToken cancellationToken = new())
        {
            var updateCount = 0;
            foreach (var accountToUpdate in accountsToUpdate)
            {
                var account = await _accountRepository.GetByIdAsync(accountToUpdate.Id).ConfigureAwait(false);
                if (account == null || !accountToUpdate.OrganizationId.HasValue)
                {
                    continue;
                }

                account.OrganizationId = accountToUpdate.OrganizationId;
                await _accountRepository.UpdateAsync(account).ConfigureAwait(false);
                updateCount++;
            }
            return updateCount;
        }
    }
}
