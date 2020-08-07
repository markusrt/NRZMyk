using AutoMapper;
using NRZMyk.Server.Controllers.ClinicalBreakpoints;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateClinicalBreakpointRequest, ClinicalBreakpoint>();
        }
    }
}
