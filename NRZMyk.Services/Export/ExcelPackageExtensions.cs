using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace NRZMyk.Services.Export
{
    public static class ExcelPackageExtensions
    {
        public static void AddSheet<T>(this ExcelPackage package, string title, ExportDefinition<T> exportDefinition, IReadOnlyList<T> entries)
        {
            var worksheet = package.Workbook.Worksheets.Add(title);
            var dataTable = exportDefinition.ToDataTable(entries);

            worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
           
            var tbl = worksheet.Tables.Add(new ExcelAddressBase(1, 1, dataTable.Rows.Count +1,  dataTable.Columns.Count), title.Replace(" ", "_"));
            tbl.ShowHeader = true;
            tbl.TableStyle = TableStyles.Light1;
            tbl.ShowTotal = false;
            
            worksheet.Cells[1, 1, entries.Count, 11].AutoFitColumns();
        }
    }
}