using System;
using System.Collections.Generic;
using NRZMyk.Services.Data.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    public class Utils
    {
        public static ExcelPackage CreateExcelPackage(IReadOnlyList<SentinelEntry> entries)
        {
            var package = new ExcelPackage();
            package.Workbook.Properties.Title = "Test Report";
            var worksheet = package.Workbook.Worksheets.Add("Sentinel Entries");

            worksheet.Cells["A1"].LoadFromCollection(entries, true);
           
            var tbl = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: entries.Count, toColumn: 11), "Data");
            tbl.ShowHeader = true;
            tbl.TableStyle = TableStyles.Light1;
            tbl.ShowTotal = true;
            
            worksheet.Cells[1, 1, entries.Count, 11].AutoFitColumns();
            return package;
        }
    }
}