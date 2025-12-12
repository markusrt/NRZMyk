using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Export
{
    public class ExportDefinitionTests
    {
        [Test]
        public void Ctor_DoesNotThrow()
        {
            var sut = CreateExportDefinition();

            sut.Should().NotBeNull();
        }

        [Test]
        public void ToDataTable_IncludesSingleEntryAndField()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    HeightInCentimeters = 185, 
                    BirthDate = new DateTime(1980, 10, 10),
                    Name = "Test"
                }
            };
            var sut = CreateExportDefinition();
            sut.AddField(person => person.HeightInCentimeters);

            var dataTable = sut.ToDataTable(persons);

            dataTable.Rows.Count.Should().Be(1);
            dataTable.Columns.Count.Should().Be(1);
            dataTable.Rows[0][0].ToString().Should().Be("185");
        }

        [Test]
        public void ToDataTable_IncludesMultipleEntriesAndFields()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    HeightInCentimeters = 185,
                    BirthDate = new DateTime(1980, 10, 10),
                    Name = "Test 1"
                },
                new Person
                {
                    HeightInCentimeters = 175,
                    BirthDate = new DateTime(1982, 11, 23),
                    Name = "Test 2"
                }
            };
            var sut = CreateExportDefinition();
            sut.AddField(person => person.HeightInCentimeters);
            sut.AddField(person => person.BirthDate);
            sut.AddField(person => person.Name);

            var dataTable = sut.ToDataTable(persons);

            dataTable.Rows.Count.Should().Be(2);
            dataTable.Columns.Count.Should().Be(3);

            dataTable.Rows[1][0].ToString().Should().Be("175");
            dataTable.Rows[1][1].Should().Be(new DateTime(1982, 11, 23));
            dataTable.Rows[1][2].ToString().Should().Be("Test 2");
        }


        [Test]
        public void ToDataTable_EnumsAreConvertedToStrings()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    AccessLevel = FileAccess.Read
                }
            };
            var sut = CreateExportDefinition();
            sut.AddField(person => person.AccessLevel);

            var dataTable = sut.ToDataTable(persons);

            dataTable.Rows.Count.Should().Be(1);
            dataTable.Columns.Count.Should().Be(1);

            dataTable.Rows[0][0].Should().Be("Read");
        }

        [Test]
        public void ToDataTable_ZeroValueIsExported()
        {
            var parents = new List<Parent>
            {
                new()
                {
                    Name = "Test Parent",
                    Child = new Child { Name = "Test Child", Age = 0 }
                }
            };
            var sut = new ParentWithAgeExportDefinition();

            var dataTable = sut.ToDataTable(parents);

            dataTable.Rows.Count.Should().Be(1);
            dataTable.Rows[0][1].ToString().Should().Be("0");
        }

        [Test]
        public void ToDataTable_NullValueReturnsEmptyString()
        {
            var parents = new List<Parent>
            {
                new()
                {
                    Name = "Test Parent",
                    Child = new Child { Name = "Test Child", HeightInCm = null }
                }
            };
            var sut = new ParentWithHeightExportDefinition();

            var dataTable = sut.ToDataTable(parents);

            dataTable.Rows.Count.Should().Be(1);
            dataTable.Rows[0][1].ToString().Should().Be("");
        }

        [Test]
        public void ToDataTable_NullableEnumReturnsEmptyString()
        {
            var parents = new List<Parent>
            {
                new()
                {
                    Name = "Test Parent",
                    Child = new Child { Name = "Test Child", NullableGender = null }
                }
            };
            var sut = new ParentWithNullableGenderExportDefinition();

            var dataTable = sut.ToDataTable(parents);

            dataTable.Rows.Count.Should().Be(1);
            dataTable.Rows[0][1].ToString().Should().Be("");
        }

        [Test]
        public void ToDataTable_DefaultEnumValueExportsDescription()
        {
            var parents = new List<Parent>
            {
                new()
                {
                    Name = "Test Parent",
                    Child = new Child { Name = "Test Child", Gender = Gender.NotStated }
                }
            };
            var sut = new ParentExportDefinition();

            var dataTable = sut.ToDataTable(parents);

            dataTable.Rows.Count.Should().Be(1);
            dataTable.Rows[0][2].Should().Be("keine Angabe");
        }
        
        [Test]
        public void ToDataTable_ChildPropertiesAreExported()
        {
            var parents = new List<Parent>
            {
                new()
                {
                    Name = "Charles, Prince of Wales",
                    Child = new Child {Name = "William, Prince of Wales", Gender = Gender.Male}
                },
                new()
                {
                    Name = "Prince George of Wales",
                    Child = null
                },
                new()
                {
                    Name = "Jane Doe",
                    Child = new Child {Name = "John Doe", Gender = Gender.NotStated}
                }
            };
            var sut = new ParentExportDefinition();
            
            var dataTable = sut.ToDataTable(parents);

            dataTable.Rows.Count.Should().Be(3);
            dataTable.Columns.Count.Should().Be(3);

            dataTable.Columns[0].ColumnName.Should().Be("Parent Name");
            dataTable.Columns[1].ColumnName.Should().Be("Child Name");
            dataTable.Columns[2].ColumnName.Should().Be("Child Gender");
            dataTable.Rows[0][0].Should().Be("Charles, Prince of Wales");
            dataTable.Rows[0][1].Should().Be("William, Prince of Wales");
            dataTable.Rows[0][2].Should().Be("männlich");
            dataTable.Rows[1][0].Should().Be("Prince George of Wales");
            dataTable.Rows[1][1].Should().Be(DBNull.Value);
            dataTable.Rows[2][0].Should().Be("Jane Doe");
            dataTable.Rows[2][1].Should().Be("John Doe");
            dataTable.Rows[2][2].Should().Be("keine Angabe");
        }

        private ExportDefinition<Person> CreateExportDefinition()
        {
            return new ExportDefinition<Person>();
        }
    }

    class ParentExportDefinition : ExportDefinition<Parent>
    {
        public ParentExportDefinition()
        {
            AddField(parent => parent.Name, "Parent Name");
            AddField(parent => ExportChildProperty(parent.Child, child => child.Name), "Child Name");
            AddField(parent => ExportChildProperty(parent.Child, child => child.Gender), "Child Gender");
        }
    }

    class ParentWithAgeExportDefinition : ExportDefinition<Parent>
    {
        public ParentWithAgeExportDefinition()
        {
            AddField(parent => parent.Name, "Parent Name");
            AddField(parent => ExportChildProperty(parent.Child, child => child.Age), "Child Age");
        }
    }

    class ParentWithHeightExportDefinition : ExportDefinition<Parent>
    {
        public ParentWithHeightExportDefinition()
        {
            AddField(parent => parent.Name, "Parent Name");
            AddField(parent => ExportChildProperty(parent.Child, child => child.HeightInCm), "Child Height");
        }
    }

    class ParentWithNullableGenderExportDefinition : ExportDefinition<Parent>
    {
        public ParentWithNullableGenderExportDefinition()
        {
            AddField(parent => parent.Name, "Parent Name");
            AddField(parent => ExportChildProperty(parent.Child, child => child.NullableGender), "Child Nullable Gender");
        }
    }

    class Parent
    {
        public string Name { get; set; }
        public Child Child { get; set; }
    }

    class Child
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public Gender? NullableGender { get; set; }
        public int Age { get; set; }
        public int? HeightInCm { get; set; }
    }
}