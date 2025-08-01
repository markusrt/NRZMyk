using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;

namespace NRZMyk.Mocks.MockServices
{
    public class MockSentinelEntryServiceImpl : ISentinelEntryService
    {
        public static int Delay = 2000;

        private readonly IMapper _mapper;
        private readonly ILogger<MockSentinelEntryServiceImpl> _logger;

        private int _id = 1;

        private readonly List<SentinelEntry> _repository = new();

        public MockSentinelEntryServiceImpl(IMapper mapper, ILogger<MockSentinelEntryServiceImpl> logger)
        {
            _mapper = mapper;
            _logger = logger;

            _repository.Add(new SentinelEntry
            {
                Id = _id++,
                Year = 2020,
                YearlySequentialEntryNumber = _id,
                AgeGroup = AgeGroup.ElevenToFifteen,
                IdentifiedSpecies = Species.CandidaDubliniensis,
                Material = Material.CentralBloodCultureCvc,
                Remark = "Some notes",
                HospitalDepartmentType = HospitalDepartmentType.NormalUnit,
                HospitalDepartment =  HospitalDepartment.Neurology,
                SamplingDate = new DateTime(2020,5,1),
                CryoDate = new DateTime(2020,10,11),
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
            _repository.Add(new SentinelEntry
            {
                Id = _id++,
                Year = 2020,
                YearlySequentialEntryNumber = _id,
                AgeGroup = AgeGroup.ElevenToFifteen,
                IdentifiedSpecies = Species.CandidaGuilliermondii,
                Material = Material.CentralBloodCulturePort,
                HospitalDepartmentType = HospitalDepartmentType.NormalUnit,
                HospitalDepartment = HospitalDepartment.Neurology,
                SamplingDate = new DateTime(2020, 5, 1),
                SenderLaboratoryNumber = "SLO-123456",
                ProtectKey = "2"
            });
        }

        public Task<SentinelEntry> Create(SentinelEntryRequest createRequest)
        {
            _logger.LogInformation("Create sentinel entry: {request}", createRequest);
            var sentinelEntry = _mapper.Map<SentinelEntry>(createRequest);
            sentinelEntry.Id = _id++;
            _repository.Add(sentinelEntry);
            return Task.FromResult(sentinelEntry);
        }

        public Task<List<SentinelEntry>> ListPaged(int pageSize)
        {
            return Task.FromResult(_repository);
        }

        public async Task<List<SentinelEntry>> ListByOrganization(int organizationId)
        {
            await Task.Delay(Delay);
            return organizationId == -1
                ? _repository.ToList()
                : _repository.Where(s => s.ProtectKey == organizationId.ToString()).ToList();
        }

        public async Task<SentinelEntryResponse> GetById(int id)
        {
            await Task.Delay(Delay);
            return _mapper.Map<SentinelEntryResponse>(_repository.FirstOrDefault(e => e.Id == id));
        }

        public Task<SentinelEntry> Update(SentinelEntryRequest updateRequest)
        {
            var entry = _repository.FirstOrDefault(e => e.Id == updateRequest.Id);
            _mapper.Map(updateRequest, entry);
            return Task.FromResult(entry);
        }

        public async Task<SentinelEntry> CryoArchive(CryoArchiveRequest archiveRequest)
        {
            await Task.Delay(Delay);
            var entry = _repository.First(e => e.Id == archiveRequest.Id);
            entry.CryoRemark = archiveRequest.CryoRemark;
            entry.CryoDate = archiveRequest.CryoDate;
            return entry;
        }

        public async Task<SentinelEntry> UpdateCryoRemark(CryoRemarkUpdateRequest updateRequest)
        {
            await Task.Delay(Delay);
            var entry = _repository.First(e => e.Id == updateRequest.Id);
            entry.CryoRemark = updateRequest.CryoRemark;
            return entry;
        }

        public async Task<string> Export()
        {
            await Task.Delay(Delay);
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