using AutoMapper;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SentinelEntryRequest, SentinelEntry>();
            CreateMap<SentinelEntry, SentinelEntryResponse>();
            //CreateMap <SentinelEntryRequest, SentinelEntryResponse>();
            CreateMap<SentinelEntryResponse, SentinelEntryRequest>();
            CreateMap<AntimicrobialSensitivityTestRequest, AntimicrobialSensitivityTest>();
            CreateMap<AntimicrobialSensitivityTest, AntimicrobialSensitivityTestRequest>().ForMember(
                dest => dest.Standard,
                opt => opt.MapFrom((source, dest) => source.ClinicalBreakpoint?.Standard ?? BrothMicrodilutionStandard.Eucast
            ));
        }
    }
}
