using System;
using System.IO;

namespace NRZMyk.Services.Tests.Export
{
    internal class Person
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int HeightInCentimeters { get; set; }
        public FileAccess AccessLevel { get; set; }
    }
}