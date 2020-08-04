using AutoMapper;
using NRZMyk.Server.Controllers.ClinicalBreakpoints;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateClinicalBreakpointRequest, ClinicalBreakpoint>();
            CreateMap<CreateSentinelEntryRequest, SentinelEntry>();
        }
    }
}
