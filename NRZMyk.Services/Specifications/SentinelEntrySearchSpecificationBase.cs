using System.Linq;
using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    /// <summary>
    /// Shared base for sentinel entry search specifications. Applies the organization
    /// (<see cref="ProtectKey"/>) constraint and a single OR-combined free-text predicate
    /// that covers every column shown in the sentinel entries table:
    /// <list type="bullet">
    ///   <item>Free text columns: <see cref="SentinelEntry.SenderLaboratoryNumber"/>,
    ///   <see cref="SentinelEntry.OtherIdentifiedSpecies"/>,
    ///   <see cref="SentinelEntry.OtherMaterial"/> and
    ///   <see cref="SentinelEntry.OtherHospitalDepartment"/>.</item>
    ///   <item>Enum columns matched by their <c>[Description]</c> values:
    ///   <see cref="Material"/>, <see cref="AgeGroup"/>, <see cref="Species"/>,
    ///   <see cref="HospitalDepartment"/> and <see cref="InternalHospitalDepartmentType"/>.</item>
    ///   <item>The constructed sentinel laboratory number (<c>SN-{Year}-{YearlySequentialEntryNumber:0000}</c>),
    ///   parsed back into year/sequence components so the search translates to integer
    ///   comparisons against the underlying columns.</item>
    /// </list>
    /// All predicates are pushed down into the database query via Ardalis.Specification.
    /// </summary>
    public abstract class SentinelEntrySearchSpecificationBase : Specification<SentinelEntry>
    {
        public string ProtectKey { get; }

        public string SearchTerm { get; }

        protected SentinelEntrySearchSpecificationBase(string protectKey, string searchTerm = null)
        {
            ProtectKey = protectKey;
            SearchTerm = searchTerm;

            if (!string.IsNullOrEmpty(protectKey))
            {
                Query.Where(s => s.ProtectKey == protectKey);
            }

            var parsed = SentinelEntrySearchTerm.Parse(searchTerm);
            if (parsed.IsEmpty)
            {
                return;
            }

            var term = parsed.NormalizedTerm;
            var materials = parsed.MaterialMatches;
            var ageGroups = parsed.AgeGroupMatches;
            var species = parsed.SpeciesMatches;
            var departments = parsed.HospitalDepartmentMatches;
            var internalDepartments = parsed.InternalHospitalDepartmentMatches;
            var exactYear = parsed.ExactYear;
            var exactSeq = parsed.ExactSequenceNumber;
            var candidateYear = parsed.CandidateYear;
            var candidateSeq = parsed.CandidateSequenceNumber;

            Query.Where(s =>
                (s.SenderLaboratoryNumber != null && s.SenderLaboratoryNumber.ToLower().Contains(term)) ||
                (s.OtherIdentifiedSpecies != null && s.OtherIdentifiedSpecies.ToLower().Contains(term)) ||
                (s.OtherMaterial != null && s.OtherMaterial.ToLower().Contains(term)) ||
                (s.OtherHospitalDepartment != null && s.OtherHospitalDepartment.ToLower().Contains(term)) ||
                materials.Contains(s.Material) ||
                ageGroups.Contains(s.AgeGroup) ||
                species.Contains(s.IdentifiedSpecies) ||
                departments.Contains(s.HospitalDepartment) ||
                internalDepartments.Contains(s.InternalHospitalDepartmentType) ||
                (exactYear.HasValue && exactSeq.HasValue
                    && s.Year == exactYear.Value
                    && s.YearlySequentialEntryNumber == exactSeq.Value) ||
                (candidateYear.HasValue && s.Year == candidateYear.Value) ||
                (candidateSeq.HasValue && s.YearlySequentialEntryNumber == candidateSeq.Value));
        }

        protected void OrderByNewest()
        {
            Query.OrderByDescending(s => s.Id);
        }
    }
}
