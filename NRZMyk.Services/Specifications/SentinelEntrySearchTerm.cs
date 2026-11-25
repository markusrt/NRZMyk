using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    /// <summary>
    /// Parses a user-supplied search term into the building blocks needed to query
    /// <see cref="SentinelEntry"/> records by every field that is shown in the sentinel
    /// entries table. This includes:
    /// <list type="bullet">
    ///   <item>A normalized lowercase fragment for substring matching on text columns.</item>
    ///   <item>Sets of enum values whose <see cref="DescriptionAttribute"/> matches the term
    ///   for <see cref="Material"/>, <see cref="AgeGroup"/>, <see cref="Species"/>,
    ///   <see cref="HospitalDepartment"/> and <see cref="InternalHospitalDepartmentType"/>.</item>
    ///   <item>Year and sequence number components for the constructed
    ///   <c>SN-{Year}-{YearlySequentialEntryNumber:0000}</c> laboratory number.</item>
    /// </list>
    /// All matching is performed against materialized enum metadata so that the resulting
    /// specifications can be translated to a single SQL <c>WHERE</c> clause, without
    /// loading entries into memory.
    /// </summary>
    public sealed class SentinelEntrySearchTerm
    {
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

        private static readonly Regex YearAndSequencePattern = new(
            @"^(?:sn[\s-]*)?(\d{4})\s*-\s*(\d{1,4})$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled,
            RegexTimeout);

        private static readonly Regex YearWithPrefixPattern = new(
            @"^sn[\s-]*(\d{4})\s*-?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled,
            RegexTimeout);

        private static readonly Regex DigitsOnlyPattern = new(
            @"^(\d{1,4})$",
            RegexOptions.Compiled,
            RegexTimeout);

        private SentinelEntrySearchTerm()
        {
            MaterialMatches = Array.Empty<Material>();
            AgeGroupMatches = Array.Empty<AgeGroup>();
            SpeciesMatches = Array.Empty<Species>();
            HospitalDepartmentMatches = Array.Empty<HospitalDepartment>();
            InternalHospitalDepartmentMatches = Array.Empty<InternalHospitalDepartmentType>();
        }

        /// <summary>
        /// True when no usable search term has been provided. Specifications should skip
        /// applying any search predicates in that case.
        /// </summary>
        public bool IsEmpty { get; private init; }

        /// <summary>Lowercase, trimmed search fragment used for substring matches on text columns.</summary>
        public string NormalizedTerm { get; private init; } = string.Empty;

        public IReadOnlyCollection<Material> MaterialMatches { get; private init; }
        public IReadOnlyCollection<AgeGroup> AgeGroupMatches { get; private init; }
        public IReadOnlyCollection<Species> SpeciesMatches { get; private init; }
        public IReadOnlyCollection<HospitalDepartment> HospitalDepartmentMatches { get; private init; }
        public IReadOnlyCollection<InternalHospitalDepartmentType> InternalHospitalDepartmentMatches { get; private init; }

        /// <summary>Year component when the term was parsed as a complete <c>YYYY-NNNN</c> laboratory number.</summary>
        public int? ExactYear { get; private init; }

        /// <summary>Sequence number component when the term was parsed as a complete <c>YYYY-NNNN</c> laboratory number.</summary>
        public int? ExactSequenceNumber { get; private init; }

        /// <summary>Year-only candidate when the term was parsed as a partial laboratory number (e.g. <c>SN-2024</c>).</summary>
        public int? CandidateYear { get; private init; }

        /// <summary>Sequence-number candidate when the term consists of plain digits only.</summary>
        public int? CandidateSequenceNumber { get; private init; }

        public static SentinelEntrySearchTerm Parse(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new SentinelEntrySearchTerm { IsEmpty = true };
            }

            var trimmed = searchTerm.Trim();
            var lower = trimmed.ToLowerInvariant();

            ParseLaboratoryNumber(trimmed, out var exactYear, out var exactSeq,
                out var candidateYear, out var candidateSeq);

            return new SentinelEntrySearchTerm
            {
                NormalizedTerm = lower,
                MaterialMatches = MatchEnumDescriptions<Material>(lower),
                AgeGroupMatches = MatchEnumDescriptions<AgeGroup>(lower),
                SpeciesMatches = MatchEnumDescriptions<Species>(lower),
                HospitalDepartmentMatches = MatchEnumDescriptions<HospitalDepartment>(lower),
                InternalHospitalDepartmentMatches = MatchEnumDescriptions<InternalHospitalDepartmentType>(lower),
                ExactYear = exactYear,
                ExactSequenceNumber = exactSeq,
                CandidateYear = candidateYear,
                CandidateSequenceNumber = candidateSeq,
            };
        }

        private static IReadOnlyCollection<TEnum> MatchEnumDescriptions<TEnum>(string lowerSearchTerm)
            where TEnum : struct, Enum
        {
            var matches = new List<TEnum>();
            foreach (var value in Enum.GetValues(typeof(TEnum)))
            {
                var description = GetDescription((Enum)value);
                if (!string.IsNullOrEmpty(description) &&
                    description.IndexOf(lowerSearchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    matches.Add((TEnum)value);
                }
            }
            return matches;
        }

        private static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? string.Empty;
        }

        private static void ParseLaboratoryNumber(string term,
            out int? exactYear, out int? exactSequence,
            out int? candidateYear, out int? candidateSequence)
        {
            exactYear = null;
            exactSequence = null;
            candidateYear = null;
            candidateSequence = null;

            var fullMatch = YearAndSequencePattern.Match(term);
            if (fullMatch.Success)
            {
                exactYear = int.Parse(fullMatch.Groups[1].Value);
                exactSequence = int.Parse(fullMatch.Groups[2].Value);
                return;
            }

            var yearOnlyMatch = YearWithPrefixPattern.Match(term);
            if (yearOnlyMatch.Success)
            {
                candidateYear = int.Parse(yearOnlyMatch.Groups[1].Value);
                return;
            }

            var digitsOnlyMatch = DigitsOnlyPattern.Match(term);
            if (digitsOnlyMatch.Success)
            {
                var number = int.Parse(digitsOnlyMatch.Groups[1].Value);
                candidateSequence = number;
                if (digitsOnlyMatch.Groups[1].Value.Length == 4)
                {
                    candidateYear = number;
                }
            }
        }
    }
}
