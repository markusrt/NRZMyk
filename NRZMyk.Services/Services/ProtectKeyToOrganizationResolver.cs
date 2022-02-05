using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;

namespace NRZMyk.Services.Services
{
    public class ProtectKeyToOrganizationResolver : IProtectKeyToOrganizationResolver
    {
        private readonly Dictionary<string,string> _cache = new Dictionary<string, string>();

        private readonly ILogger<ProtectKeyToOrganizationResolver> _logger;
        private readonly IAsyncRepository<Organization> _organizationRepository;

        public ProtectKeyToOrganizationResolver(ILogger<ProtectKeyToOrganizationResolver> logger, IAsyncRepository<Organization> organizationRepository)
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
        }

        public async Task<string> ResolveOrganization(string protectKey)
        {
            if (_cache.TryGetValue(protectKey, out var organizationName))
            {
                return organizationName;
            }

            try
            {
                var organization = await _organizationRepository.GetByIdAsync(int.Parse(protectKey)).ConfigureAwait(false);
                organizationName = organization.Name;
                _cache.Add(protectKey, organizationName);
                return organizationName;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to resolve organization with protect key '{protectKey}'");
                return string.Empty;
            }
        }
    }
}