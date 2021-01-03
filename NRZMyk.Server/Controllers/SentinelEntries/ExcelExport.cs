using System;
using System.Globalization;
using System.Threading.Tasks;
using HaemophilusWeb.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
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

        public ExcelExport(IAsyncRepository<SentinelEntry> sentinelEntryRepository, IProtectKeyToOrganizationResolver organizationResolver)
        {
            _sentinelEntryRepository = sentinelEntryRepository;
            _organizationResolver = organizationResolver;
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
            var export = new SentinelEntryExportDefinition(_organizationResolver);
            using(var package = ExcelUtils.CreateExcelPackage(export, await _sentinelEntryRepository.ListAllAsync()))
            {
                reportBytes = package.GetAsByteArray();
            }
            return File(reportBytes, XlsxContentType, $"Sentinel-Export_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
