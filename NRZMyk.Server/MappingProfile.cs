using AutoMapper;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateSentinelEntryRequest, SentinelEntry>();
        }
    }
}
