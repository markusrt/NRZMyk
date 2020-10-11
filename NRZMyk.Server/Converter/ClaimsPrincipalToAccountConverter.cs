using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.Graph;
using Newtonsoft.Json;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Server.Converter
{
    public class ClaimsPrincipalToAccountConverter : ITypeConverter<ClaimsPrincipal, RemoteAccount>
    {
        public RemoteAccount Convert(ClaimsPrincipal source, RemoteAccount destination, ResolutionContext context)
        {
            destination ??= new RemoteAccount();
            destination.DisplayName = source.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            destination.Street = source.Claims.FirstOrDefault(c => c.Type == "streetAddress")?.Value;
            destination.Postalcode = source.Claims.FirstOrDefault(c => c.Type == "postalCode")?.Value;
            destination.City = source.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
            destination.Country = source.Claims.FirstOrDefault(c => c.Type == "country")?.Value;
            
            var objectId = source.Claims.FirstOrDefault(c => c.Type == "oid" || c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            destination.ObjectId = string.IsNullOrEmpty(objectId) ? Guid.Empty : Guid.Parse(objectId);

            var emails = source.Claims.FirstOrDefault(c => c.Type == "emails")?.Value ?? "[]";
            var hasSingleEmail = !(emails.Trim().StartsWith("[") && emails.Trim().EndsWith("]"));
            destination.Email = hasSingleEmail 
                ? emails 
                : JsonConvert.DeserializeObject<List<string>>(emails).FirstOrDefault();

            return destination;
        }
    }
}