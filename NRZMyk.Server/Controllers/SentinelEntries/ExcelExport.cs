using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HaemophilusWeb.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using OfficeOpenXml;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [ApiController]
    [Authorize(Roles = nameof(Role.SuperUser))]
    public class ExcelExport : ControllerBase
    {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;
        private readonly IProtectKeyToOrganizationResolver _organizationResolver;
        private readonly MicStepsService _micStepsService;

        public ExcelExport(IAsyncRepository<SentinelEntry> sentinelEntryRepository, IProtectKeyToOrganizationResolver organizationResolver, MicStepsService micStepsService)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _organizationResolver = organizationResolver;
            _micStepsService = micStepsService;
        }

        [HttpGet("api/sentinel-entries/export")]
        [SwaggerOperation(
            Summary = "Export all sentinel entries to excel",
            OperationId = "sentinel-entries.ListPaged",
            Tags = new[] { "SentinelEndpoints" })
        ]
        public async Task<IActionResult> DownloadExcel()
        {
            byte[] reportBytes;
            var entriesExport = new SentinelEntryExportDefinition(_organizationResolver);
            var testsExport = new AntimicrobialSensitivityTestExportDefinition(_micStepsService);
            
            using(var package = new ExcelPackage())
            {
                var entries = await _sentinelEntryRepository.ListAsync(new SentinelEntriesIncludingTestsSpecification());
                package.AddSheet("Sentinel Daten", entriesExport, entries);
                package.AddSheet("Resistenztestung", testsExport, entries.SelectMany(e => e.AntimicrobialSensitivityTests).ToList());
                reportBytes = await package.GetAsByteArrayAsync();
            }
            return File(reportBytes, XlsxContentType, $"Sentinel-Export_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
