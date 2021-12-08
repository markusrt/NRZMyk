using FluentAssertions;
using NRZMyk.Services.Export;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace NRZMyk.Services.Tests.Export
{
    public class ExcelPackageExtensionsTests
    {
        
        private readonly ExportDefinition<Person> _exportDefinition = new ExportDefinition<Person>();

        private readonly IReadOnlyList<Person> _data = new List<Person>
        {
            new Person
            {
                HeightInCentimeters = 185,
                BirthDate = new DateTime(1980, 10, 10),
                Name = "Test 1"
            }
        };

        private ExcelPackage _sut;

        [OneTimeSetUp]
        public void SetupExportDefinition()
        {
            _exportDefinition.AddField(person => person.HeightInCentimeters);
            _exportDefinition.AddField(person => person.BirthDate);
            _exportDefinition.AddField(person => person.Name);
        }

        [SetUp]
        public void Setup()
        {
            _sut = new ExcelPackage();
        }

        [TearDown]
        public void TearDown()
        {
            _sut.Dispose();
        }

        [Test]
        public void AddSheet_AddsTable()
        {
            _sut.AddSheet("Title", _exportDefinition, _data);

            _sut.Workbook.Worksheets.Should().HaveCount(1);
            _sut.Workbook.Worksheets[0].Tables.Should().HaveCount(1);
        }

        [Test]
        public void AddSheet_SetsTableProperties()
        {
            _sut.AddSheet("My Title", _exportDefinition, _data);

            var table = _sut.Workbook.Worksheets[0].Tables[0];
            table.ShowHeader.Should().BeTrue();
            table.ShowTotal.Should().BeFalse();
            table.TableStyle.Should().Be(TableStyles.Light1);
            table.Name.Should().Be("My_Title");
        }
    }
}