using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
   
    public class AccountService : IAccountService
    {
        private readonly IHttpClient _httpClient;

        public AccountService(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<ICollection<RemoteAccount>> ListAccounts()
            => _httpClient.Get<ICollection<RemoteAccount>>("api/users");

        public Task<ICollection<Organization>> ListOrganizations()
            => _httpClient.Get<ICollection<Organization>>("api/organizations");

        public Task<int> AssignToOrganization(ICollection<RemoteAccount> accounts)
            => _httpClient.Post<ICollection<RemoteAccount>, int>("api/users/assign-organization", accounts);
    }
}