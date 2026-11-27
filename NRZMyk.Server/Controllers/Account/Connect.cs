using System;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NRZMyk.Services.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.Account
{
    [Authorize]
    [Route("api/user/connect")]
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

        [HttpGet]
        [SwaggerOperation(
            Summary = "Informs about remote account connection",
            OperationId = "account.connect",
            Tags = new[] { "AccountEndpoints" })
        ]
        public override async Task<ActionResult<ConnectedAccount>> HandleAsync(CancellationToken cancellationToken = new())
        {
            var connectingAccount = MapToAccount(User);
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
                MapToAccount(User, storedAccount);
                await _accountRepository.UpdateAsync(storedAccount).ConfigureAwait(false);
            }

            return new ConnectedAccount
            {
                Account = storedAccount, 
                IsGuest = !storedAccount.OrganizationId.HasValue
            };
        }

        private RemoteAccount MapToAccount(ClaimsPrincipal user)
        {
            try
            {
                return _mapper.Map<RemoteAccount>(user);
            }
            catch (AutoMapperMappingException)
            {
                return FallbackMapToAccount(user, new RemoteAccount());
            }
        }

        private RemoteAccount MapToAccount(ClaimsPrincipal user, RemoteAccount destination)
        {
            try
            {
                return _mapper.Map(user, destination);
            }
            catch (AutoMapperMappingException)
            {
                return FallbackMapToAccount(user, destination);
            }
        }

        private static RemoteAccount FallbackMapToAccount(ClaimsPrincipal user, RemoteAccount destination)
        {
            destination.DisplayName = user.Claims.Name();
            destination.Street = user.Claims.Address();
            destination.Postalcode = user.Claims.Postalcode();
            destination.City = user.Claims.City();
            destination.Country = user.Claims.Country();
            destination.ObjectId = TryGetObjectIdOrEmpty(user.Claims);
            destination.Email = TryGetFirstEmailOrDefault(user.Claims);
            return destination;
        }

        private static Guid TryGetObjectIdOrEmpty(IEnumerable<Claim> claims)
        {
            try
            {
                return claims.ObjectId();
            }
            catch
            {
                return Guid.Empty;
            }
        }

        private static string TryGetFirstEmailOrDefault(IEnumerable<Claim> claims)
        {
            try
            {
                return claims.FirstEmail();
            }
            catch
            {
                return null;
            }
        }
    }
}
