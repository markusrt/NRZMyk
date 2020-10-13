using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using NRZMyk.Services.Models;
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Services.Utils
{
    public static class ClaimsExtensions
    {
        public static string Name(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == "name")?.Value;
        }

        public static string Address(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == "streetAddress")?.Value;
        }

        public static string Postalcode(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == "postalCode")?.Value;
        }

        public static string City(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == "city")?.Value;
        }

        public static string OrganizationId(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Organization)?.Value;
        }

        public static string Country(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == "country")?.Value;
        }

        public static Guid ObjectId(this IEnumerable<Claim> claims)
        {
            var objectId = claims.FirstOrDefault(c => c.Type == "oid" || c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            return string.IsNullOrEmpty(objectId) ? Guid.Empty : Guid.Parse(objectId);
        }

        public static string FirstEmail(this IEnumerable<Claim> claims)
        {
            var emails = claims.FirstOrDefault(c => c.Type == "emails")?.Value ?? "[]";
            var hasSingleEmail = !(emails.Trim().StartsWith("[") && emails.Trim().EndsWith("]"));
            return hasSingleEmail 
                ? emails 
                : JsonSerializer.Deserialize<List<string>>(emails).FirstOrDefault();
        }
    }
}
