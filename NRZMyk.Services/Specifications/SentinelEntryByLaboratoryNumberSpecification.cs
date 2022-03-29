using System.Text.RegularExpressions;
using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntryByLaboratoryNumberSpecification : SentinelEntryFilterSpecification
    {
        private static readonly Regex LaboratoryNumberFormat = new Regex("SN-(\\d\\d\\d\\d)-(\\d\\d\\d\\d)", RegexOptions.Compiled);

        public int Year { get; }

        public int SequentialNumber { get; }

        public SentinelEntryByLaboratoryNumberSpecification(string laboratoryNumber, string protectKey) : base(protectKey)
        {
            Year = ParseYear(laboratoryNumber);
            SequentialNumber = ParseSequentialNumber(laboratoryNumber);

            AddCriteria(s => s.Year == Year && s.YearlySequentialEntryNumber == SequentialNumber);
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
