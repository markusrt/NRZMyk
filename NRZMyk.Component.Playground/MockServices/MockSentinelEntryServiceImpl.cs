using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Playground.MockServices
{
    public class MockSentinelEntryServiceImpl : SentinelEntryService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<MockSentinelEntryServiceImpl> _logger;

        private readonly List<SentinelEntry> _repository = new List<SentinelEntry>();

        public MockSentinelEntryServiceImpl(IMapper mapper, ILogger<MockSentinelEntryServiceImpl> logger)
        {
            _mapper = mapper;
            _logger = logger;

            _repository.Add(new SentinelEntry
            {
                AgeGroup = AgeGroup.ElevenToFifteen,
                IdentifiedSpecies = "Identified species",
                Material = Material.Isolate,
                Remark = "Some notes",
                ResidentialTreatment = ResidentialTreatment.MixedIntensiveCareUnit,
                SamplingDate = new DateTime(2020,5,1),
                SenderLaboratoryNumber = "SLN-123456",
                SpeciesTestingMethod = SpeciesTestingMethod.Vitek,
            });
        }

        public Task<SentinelEntry> Create(CreateSentinelEntryRequest createRequest)
        {
            _logger.LogInformation($"Create sentinel entry: {createRequest}");
            var sentinelEntry = _mapper.Map<SentinelEntry>(createRequest);
            _repository.Add(sentinelEntry);
            return Task.FromResult(sentinelEntry);
        }

        public Task<List<SentinelEntry>> ListPaged(int pageSize)
        {
            return Task.FromResult(_repository);
        }
    }
}