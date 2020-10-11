using System.Security.Claims;
using AutoMapper;
using NRZMyk.Server.Controllers.ClinicalBreakpoints;
using NRZMyk.Server.Converter;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateClinicalBreakpointRequest, ClinicalBreakpoint>();
            CreateMap<ClaimsPrincipal, RemoteAccount>().ConvertUsing<ClaimsPrincipalToAccountConverter>();
        }
    }
}
