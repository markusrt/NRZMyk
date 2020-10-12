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

namespace NRZMyk.Server.Controllers.Account
{
    [Authorize]
    public class Connect : BaseAsyncEndpoint<ConnectedAccount>
    {
        private readonly IAsyncRepository<RemoteAccount> _accountRepository;
        private readonly IMapper _mapper;

        public Connect(IAsyncRepository<RemoteAccount> accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        [HttpGet("api/user/connect")]
        [SwaggerOperation(
            Summary = "Informs about remote account connection",
            OperationId = "account.connect",
            Tags = new[] { "AccountEndpoints" })
        ]
        public override async Task<ActionResult<ConnectedAccount>> HandleAsync()
        {
            var connectingAccount = _mapper.Map<RemoteAccount>(User);
            var storedAccount = await _accountRepository.FirstOrDefaultAsync(
                new RemoteAccountByObjectIdSpecification(connectingAccount.ObjectId));
            if (storedAccount == null)
            {
                storedAccount = await _accountRepository.AddAsync(connectingAccount);
            }
            else
            {
                _mapper.Map(User, storedAccount);
                await _accountRepository.UpdateAsync(storedAccount);
            }

            return new ConnectedAccount
            {
                Account = storedAccount, 
                IsGuest = true
            };
        }
    }
}
