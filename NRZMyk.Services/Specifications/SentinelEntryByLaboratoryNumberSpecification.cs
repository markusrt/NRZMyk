using System;
using System.Numerics;
using System.Text.RegularExpressions;
using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntryByLaboratoryNumberSpecification : Specification<SentinelEntry>
    {
        private static readonly Regex LaboratoryNumberFormat = 
            new("SN-(\\d\\d\\d\\d)-(\\d\\d\\d\\d)", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        public int Year { get; }

        public int SequentialNumber { get; }

        public string ProtectKey { get; }

        public SentinelEntryByLaboratoryNumberSpecification(string laboratoryNumber, string protectKey)
        {
            Year = ParseYear(laboratoryNumber);
            SequentialNumber = ParseSequentialNumber(laboratoryNumber);
            ProtectKey = protectKey;
            Query
                .Where(s => s.ProtectKey == protectKey)
                .Where(s => s.Year == Year && s.YearlySequentialEntryNumber == SequentialNumber)
                .OrderByDescending(s => s.Id);
        }

        private static int ParseYear(string laboratoryNumber)
        {
            int.TryParse(LaboratoryNumberFormat.Match(laboratoryNumber).Groups[1].Value, out var year);
            return year;
        }

        private static int ParseSequentialNumber(string laboratoryNumber)
        {
            int.TryParse(LaboratoryNumberFormat.Match(laboratoryNumber).Groups[2].Value, out var sequentialNumber);
            return sequentialNumber;
        }
    }
}
