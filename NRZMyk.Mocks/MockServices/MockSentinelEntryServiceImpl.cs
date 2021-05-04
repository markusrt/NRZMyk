using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;

namespace NRZMyk.Mocks.MockServices
{
    public class MockSentinelEntryServiceImpl : SentinelEntryService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<MockSentinelEntryServiceImpl> _logger;

        private int _id = 1;

        private readonly List<SentinelEntry> _repository = new List<SentinelEntry>();

        public MockSentinelEntryServiceImpl(IMapper mapper, ILogger<MockSentinelEntryServiceImpl> logger)
        {
            _mapper = mapper;
            _logger = logger;

            _repository.Add(new SentinelEntry
            {
                Id = _id++,
                AgeGroup = AgeGroup.ElevenToFifteen,
                IdentifiedSpecies = Species.CandidaDubliniensis,
                Material = Material.CentralBloodCultureCvc,
                Remark = "Some notes",
                HospitalDepartmentType = HospitalDepartmentType.NormalUnit,
                HospitalDepartment =  HospitalDepartment.Neurology,
                SamplingDate = new DateTime(2020,5,1),
                SenderLaboratoryNumber = "SLN-123456",
                ProtectKey = "1",
                AntimicrobialSensitivityTests = new List<AntimicrobialSensitivityTest>
                {
                    new AntimicrobialSensitivityTest
                    {
                        AntifungalAgent = AntifungalAgent.Fluconazole,
                        TestingMethod = SpeciesTestingMethod.Vitek,
                        ClinicalBreakpoint = new ClinicalBreakpoint()
                        {
                            Standard = BrothMicrodilutionStandard.Eucast,
                            Id = 226
                        },
                        Resistance = Resistance.Intermediate,
                        MinimumInhibitoryConcentration = 0.12f
                    }
                }
            });
        }

        public Task<SentinelEntry> Create(SentinelEntryRequest request)
        {
            _logger.LogInformation($"Create sentinel entry: {request}");
            var sentinelEntry = _mapper.Map<SentinelEntry>(request);
            sentinelEntry.Id = _id++;
            _repository.Add(sentinelEntry);
            return Task.FromResult(sentinelEntry);
        }

        public Task<List<SentinelEntry>> ListPaged(int pageSize)
        {
            return Task.FromResult(_repository);
        }

        public Task<List<SentinelEntry>> ListByOrganization(int organizationId)
        {
            return Task.FromResult(_repository.Where(s => s.ProtectKey == organizationId.ToString()).ToList());
        }

        public Task<SentinelEntryRequest> GetById(int id)
        {
            var entry = _repository.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(_mapper.Map<SentinelEntryRequest>(entry));
        }

        public Task<SentinelEntry> Update(SentinelEntryRequest updateRequest)
        {
            var entry = _repository.FirstOrDefault(e => e.Id == updateRequest.Id);
            _mapper.Map(updateRequest, entry);
            return Task.FromResult(entry);
        }

        public Task<SentinelEntry> CryoArchive(CryoArchiveRequest request)
        {
            var entry = _repository.FirstOrDefault(e => e.Id == request.Id);
            entry.CryoRemark = request.CryoRemark;
            entry.CryoDate = request.CryoDate;
            return Task.FromResult(entry);
        }

        public async Task<string> Export()
        {
            await Task.Delay(2500);
            await using var stream = GetType().Assembly.GetManifestResourceStream("NRZMyk.Mocks.Data.Export.xlsx");
            var data = new byte[stream.Length];
            await stream.ReadAsync(data, 0, data.Length);
            return Convert.ToBase64String(data);
        }

        public Task<List<string>> Other(string other)
        {
            return Task.FromResult(new List<string> {$"{other} 1",$"{other} 2", $"{other} 3"});
        }

        public Task Delete(int id)
        {
            var entry = _repository.FirstOrDefault(e => e.Id == id);
            _repository.Remove(entry);
            return Task.CompletedTask;
        }
    }
}