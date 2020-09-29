using System;
using System.Globalization;
using System.Threading.Tasks;
using HaemophilusWeb.Tools;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;
using NRZMyk.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    [ApiController]
    public class ExcelExport : ControllerBase
    {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        
        private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;

        public ExcelExport(IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
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
            var export = new SentinelEntryExportDefinition();
            using(var package = ExcelUtils.CreateExcelPackage(export, await _sentinelEntryRepository.ListAllAsync()))
            {
                reportBytes = package.GetAsByteArray();
            }
            return File(reportBytes, XlsxContentType, $"Sentinel-Export_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
