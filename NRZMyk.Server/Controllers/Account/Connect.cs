﻿using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.Account
{
    [Authorize]
    public class Connect : EndpointBaseAsync.WithoutRequest.WithActionResult<ConnectedAccount>
    {
        private readonly IAsyncRepository<RemoteAccount> _accountRepository;
        private readonly IMapper _mapper;
        private readonly IEmailNotificationService _emailNotificationService;

        public Connect(IAsyncRepository<RemoteAccount> accountRepository, IMapper mapper, IEmailNotificationService emailNotificationService)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _emailNotificationService = emailNotificationService;
        }

        [HttpGet("api/user/connect")]
        [SwaggerOperation(
            Summary = "Informs about remote account connection",
            OperationId = "account.connect",
            Tags = new[] { "AccountEndpoints" })
        ]
        public override async Task<ActionResult<ConnectedAccount>> HandleAsync(CancellationToken cancellationToken = new())
        {
            var connectingAccount = _mapper.Map<RemoteAccount>(User);
            var storedAccount = await _accountRepository.FirstOrDefaultAsync(
                new RemoteAccountByObjectIdSpecification(connectingAccount.ObjectId));
            if (storedAccount == null)
            {
                storedAccount = await _accountRepository.AddAsync(connectingAccount).ConfigureAwait(false);
                await _emailNotificationService
                    .NotifyNewUserRegistered(storedAccount.DisplayName, storedAccount.Email, storedAccount.City);
            }
            else
            {
                _mapper.Map(User, storedAccount);
                await _accountRepository.UpdateAsync(storedAccount).ConfigureAwait(false);
            }

            return new ConnectedAccount
            {
                Account = storedAccount, 
                IsGuest = !storedAccount.OrganizationId.HasValue
            };
        }
    }
}
