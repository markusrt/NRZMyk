using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.CatalogItems
{
    [Authorize]
    public class ListPaged : BaseAsyncEndpoint<ListPagedCatalogItemRequest, ListPagedCatalogItemResponse>
    {
        private readonly IAsyncRepository<CatalogItem> _itemRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IMapper _mapper;

        public ListPaged(IAsyncRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _uriComposer = uriComposer;
            _mapper = mapper;
        }

        [HttpGet("api/catalog-items")]
        [SwaggerOperation(
            Summary = "List Catalog Items (paged)",
            Description = "List Catalog Items (paged)",
            OperationId = "catalog-items.ListPaged",
            Tags = new[] { "CatalogItemEndpoints" })
        ]
        public override async Task<ActionResult<ListPagedCatalogItemResponse>> HandleAsync([FromQuery]ListPagedCatalogItemRequest request)
        {
            var response = new ListPagedCatalogItemResponse(request.CorrelationId());

            var filterSpec = new CatalogFilterSpecification(request.CatalogBrandId, request.CatalogTypeId);
            int totalItems = await _itemRepository.CountAsync(filterSpec);

            var pagedSpec = new CatalogFilterPaginatedSpecification(
                request.PageIndex * request.PageSize,
                request.PageSize,
                request.CatalogBrandId,
                request.CatalogTypeId);

            var items = await _itemRepository.ListAsync(pagedSpec);

            response.CatalogItems.AddRange(items.Select(_mapper.Map<CatalogItemDto>));
            foreach (CatalogItemDto item in response.CatalogItems)
            {
                item.PictureUri = _uriComposer.ComposePicUri(item.PictureUri);
            }
            response.PageCount = int.Parse(Math.Ceiling((decimal)totalItems / request.PageSize).ToString());

            return Ok(response);
        }
    }
}
