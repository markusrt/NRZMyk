using System;
using FluentAssertions;
using NRZMyk.Services.Utils;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Utils;

public class ReportFormatterTests
{
    [Test]
    public void ToReportFormat_NullDate_ReturnsNonEmptyString()
    {
        DateTime? date = null;

        date.ToReportFormat().Should().Be("keine Angabe");
        date.ToReportFormatMonthYear().Should().Be("keine Angabe");
        date.ToReportFormat("-").Should().Be("-");
        date.ToReportFormatMonthYear(null).Should().Be(null);
    }

    [Test]
    public void ToReportFormat_NonNullDate_ReturnsFormattedString()
    {
        DateTime? date = new DateTime(2010, 10, 21);

        date.ToReportFormat().Should().Be("21.10.2010");
        date.ToReportFormatMonthYear().Should().Be("10 / 2010");
    }
}