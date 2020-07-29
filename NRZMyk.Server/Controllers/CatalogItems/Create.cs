﻿using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Data.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.CatalogItems
{

    [Authorize]
    public class Create : BaseAsyncEndpoint<CreateCatalogItemRequest, CreateCatalogItemResponse>
    {
        private readonly IAsyncRepository<CatalogItem> _itemRepository;
        private readonly IUriComposer _uriComposer;

        public Create(IAsyncRepository<CatalogItem> itemRepository, IUriComposer uriComposer)
        {
            _itemRepository = itemRepository;
            _uriComposer = uriComposer;
        }

        [HttpPost("api/catalog-items")]
        [SwaggerOperation(
            Summary = "Creates a new Catalog Item",
            Description = "Creates a new Catalog Item",
            OperationId = "catalog-items.create",
            Tags = new[] { "CatalogItemEndpoints" })
        ]
        public override async Task<ActionResult<CreateCatalogItemResponse>> HandleAsync(CreateCatalogItemRequest request)
        {
            var response = new CreateCatalogItemResponse(request.CorrelationId());

            var newItem = new CatalogItem(request.CatalogTypeId, request.CatalogBrandId, request.Description, request.Name, request.Price, request.PictureUri);

            newItem = await _itemRepository.AddAsync(newItem);

            var dto = new CatalogItemDto
            {
                Id = newItem.Id,
                CatalogBrandId = newItem.CatalogBrandId,
                CatalogTypeId = newItem.CatalogTypeId,
                Description = newItem.Description,
                Name = newItem.Name,
                PictureUri = _uriComposer.ComposePicUri(newItem.PictureUri),
                Price = newItem.Price
            };
            response.CatalogItem = dto;
            return response;
        }
    }
}
