using System;
using System.Globalization;

namespace NRZMyk.Services.Utils;

public static class ReportFormatter
{
    public static string ToReportFormat(this DateTime? dateTime)
    {
        return dateTime.ToReportFormat("keine Angabe");
    }

    public static string ToReportFormat(this DateTime? dateTime, string emptyString)
    {
        if (!dateTime.HasValue)
        {
            return emptyString;
        }
        return dateTime.Value.ToReportFormat();
    }

    public static string ToReportFormat(this DateTime dateTime)
    {
        return dateTime.ToString("dd.MM.yyyy");
    }

    public static string ToReportFormatMonthYear(this DateTime? dateTime)
    {
        return dateTime.ToReportFormatMonthYear("keine Angabe");
    }

    public static string ToReportFormatMonthYear(this DateTime? dateTime, string emptyString)
    {
        if (!dateTime.HasValue)
        {
            return emptyString;
        }
        return dateTime.Value.ToString("MM / yyyy", CultureInfo.InvariantCulture);
    }
   
}