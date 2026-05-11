using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Converter;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.Account
{
    [Authorize]
    [Route("api/user/connect")]
    public class Connect : EndpointBaseAsync.WithoutRequest.WithActionResult<ConnectedAccount>
    {
        private static readonly ClaimsPrincipalToAccountConverter ClaimsPrincipalConverter = new();
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

        private RemoteAccount MapToAccount(System.Security.Claims.ClaimsPrincipal user)
        {
            try
            {
                return _mapper.Map<RemoteAccount>(user);
            }
            catch (AutoMapperMappingException)
            {
                return ClaimsPrincipalConverter.Convert(user, null!, default!);
            }
        }

        private RemoteAccount MapToAccount(System.Security.Claims.ClaimsPrincipal user, RemoteAccount destination)
        {
            try
            {
                return _mapper.Map(user, destination);
            }
            catch (AutoMapperMappingException)
            {
                return ClaimsPrincipalConverter.Convert(user, destination, default!);
            }
        }
    }
}
