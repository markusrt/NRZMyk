using AutoMapper;
using NRZMyk.Server.Controllers.CatalogBrands;
using NRZMyk.Server.Controllers.CatalogItems;
using NRZMyk.Server.Controllers.CatalogTypes;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CatalogItem, CatalogItemDto>();
            CreateMap<CatalogType, CatalogTypeDto>()
                .ForMember(dto => dto.Name, options => options.MapFrom(src => src.Type));
            CreateMap<CatalogBrand, CatalogBrandDto>()
                .ForMember(dto => dto.Name, options => options.MapFrom(src => src.Brand));
        }
    }
}
