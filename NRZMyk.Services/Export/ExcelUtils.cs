using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace NRZMyk.Services.Export
{
    public static class ExcelUtils
    {
        public static ExcelPackage CreateExcelPackage<T>(ExportDefinition<T> exportDefinition, IReadOnlyList<T> entries)
        {
            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Sentinel Entries");
            var dataTable = exportDefinition.ToDataTable(entries);

            package.Workbook.Properties.Title = "Test Report";
            worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
           
            var tbl = worksheet.Tables.Add(new ExcelAddressBase(1, 1, dataTable.Rows.Count +1,  dataTable.Columns.Count), "Data");
            tbl.ShowHeader = true;
            tbl.TableStyle = TableStyles.Light1;
            tbl.ShowTotal = false;
            
            worksheet.Cells[1, 1, entries.Count, 11].AutoFitColumns();
            return package;
        }
    }
}