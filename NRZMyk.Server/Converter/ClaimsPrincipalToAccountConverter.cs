using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.Graph;
using Newtonsoft.Json;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Utils;

namespace NRZMyk.Server.Converter
{
    public class ClaimsPrincipalToAccountConverter : ITypeConverter<ClaimsPrincipal, RemoteAccount>
    {
        public RemoteAccount Convert(ClaimsPrincipal source, RemoteAccount destination, ResolutionContext context)
        {
            destination ??= new RemoteAccount();
            destination.DisplayName = source.Claims.Name();
            destination.Street = source.Claims.Address();
            destination.Postalcode = source.Claims.Postalcode();
            destination.City = source.Claims.City();
            destination.Country = source.Claims.Country();
            destination.ObjectId = source.Claims.ObjectId();
            destination.Email = source.Claims.FirstEmail();
            return destination;
        }
    }
}