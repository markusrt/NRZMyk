using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.CatalogBrands
{
    public class List : BaseAsyncEndpoint<IList<CatalogBrandDto>>
    {
        private readonly IAsyncRepository<CatalogBrand> _catalogBrandRepository;
        private readonly IMapper _mapper;

        public List(IAsyncRepository<CatalogBrand> catalogBrandRepository,
            IMapper mapper)
        {
            _catalogBrandRepository = catalogBrandRepository;
            _mapper = mapper;
        }

        [HttpGet("api/catalog-brands")]
        [SwaggerOperation(
            Summary = "List Catalog Brands",
            Description = "List Catalog Brands",
            OperationId = "catalog-brands.List",
            Tags = new[] { "CatalogBrandEndpoints" })
        ]
        public override async Task<ActionResult<IList<CatalogBrandDto>>> HandleAsync()
        {
            var items = await _catalogBrandRepository.ListAllAsync();

            var response = items.Select(_mapper.Map<CatalogBrandDto>);

            return Ok(response);
        }
    }
}
