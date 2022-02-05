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
        private readonly HttpClient _httpClient;
        private readonly ILogger<SentinelEntryServiceImpl> _logger;

        public AccountService(HttpClient httpClient, ILogger<SentinelEntryServiceImpl> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ICollection<RemoteAccount>> ListAccounts()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ICollection<RemoteAccount>>($"api/users").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to load accounts from backend");
                return new List<RemoteAccount>();
            }
        }

        public async Task<ICollection<Organization>> ListOrganizations()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ICollection<Organization>>($"api/organizations").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to load organizations from backend");
                return new List<Organization>();
            }
        }

        public async Task AssignToOrganizationAsync(ICollection<RemoteAccount> accounts)
        {
            try
            {
                await _httpClient.PostAsJsonAsync("api/users/assign-organization", accounts).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to assign accounts to organization");
                throw;
            }
        }
    }
}