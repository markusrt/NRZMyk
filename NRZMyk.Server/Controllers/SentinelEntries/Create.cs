﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NRZMyk.Services.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [Authorize(Roles = nameof(Role.User))]
    public class Create : BaseAsyncEndpoint<SentinelEntryRequest, SentinelEntry>
    {
        private readonly ISentinelEntryRepository _sentinelEntryRepository;
        private readonly IMapper _mapper;

        public string OrganizationId => User.Claims.OrganizationId();

        public Create(ISentinelEntryRepository sentinelEntryRepository, IMapper mapper)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _mapper = mapper;
        }

        [HttpPost("api/sentinel-entries")]
        [SwaggerOperation(
            Summary = "Creates a new Sentinel Entry",
            OperationId = "catalog-entries.create",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public override async Task<ActionResult<SentinelEntry>> HandleAsync(SentinelEntryRequest request )
        {
            if (string.IsNullOrEmpty(OrganizationId))
            {
                return Forbid();
            }

      
            var newEntry = _mapper.Map<SentinelEntry>(request);

            var error = await Utils.ResolvePredecessor(request, newEntry, _sentinelEntryRepository, OrganizationId, ModelState).ConfigureAwait(false);
            if (error)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _sentinelEntryRepository.AssignNextEntryNumber(newEntry);
            _sentinelEntryRepository.AssignNextCryoBoxNumber(newEntry);
            newEntry.ProtectKey = OrganizationId;

            var storedEntry = await _sentinelEntryRepository.AddAsync(newEntry).ConfigureAwait(false);
            return Created(new Uri($"{Request.GetUri()}/{storedEntry.Id}"), storedEntry);
        }
    }
}
