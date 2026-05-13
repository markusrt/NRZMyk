using System.Linq;
using System.Linq.Expressions;
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
        }

        protected void ApplyBaseFilters()
        {
            if (!string.IsNullOrEmpty(ProtectKey))
            {
                Query.Where(s => s.ProtectKey == ProtectKey);
            }

            var parsed = SentinelEntrySearchTerm.Parse(SearchTerm);
            if (parsed.IsEmpty)
            {
                return;
            }

            Query.Where(BuildSearchPredicate(parsed));
        }

        protected void OrderByNewest()
        {
            Query.OrderByDescending(s => s.Id);
        }

        /// <summary>
        /// Builds the OR-combined search predicate for a parsed search term. The
        /// composition is split into small sub-predicates so that each individual
        /// expression stays simple while the resulting tree still translates to a
        /// single SQL <c>WHERE</c> clause.
        /// </summary>
        internal static Expression<System.Func<SentinelEntry, bool>> BuildSearchPredicate(SentinelEntrySearchTerm parsed)
        {
            var textMatch = TextMatchPredicate(parsed.NormalizedTerm);
            var enumMatch = EnumMatchPredicate(parsed);
            var labNumberMatch = LaboratoryNumberPredicate(parsed);

            return Or(Or(textMatch, enumMatch), labNumberMatch);
        }

        private static Expression<System.Func<SentinelEntry, bool>> TextMatchPredicate(string term)
        {
            return s =>
                (s.SenderLaboratoryNumber != null && s.SenderLaboratoryNumber.ToLower().Contains(term)) ||
                (s.OtherIdentifiedSpecies != null && s.OtherIdentifiedSpecies.ToLower().Contains(term)) ||
                (s.OtherMaterial != null && s.OtherMaterial.ToLower().Contains(term)) ||
                (s.OtherHospitalDepartment != null && s.OtherHospitalDepartment.ToLower().Contains(term));
        }

        private static Expression<System.Func<SentinelEntry, bool>> EnumMatchPredicate(SentinelEntrySearchTerm parsed)
        {
            var materials = parsed.MaterialMatches;
            var ageGroups = parsed.AgeGroupMatches;
            var species = parsed.SpeciesMatches;
            var departments = parsed.HospitalDepartmentMatches;
            var internalDepartments = parsed.InternalHospitalDepartmentMatches;

            return s =>
                materials.Contains(s.Material) ||
                ageGroups.Contains(s.AgeGroup) ||
                species.Contains(s.IdentifiedSpecies) ||
                departments.Contains(s.HospitalDepartment) ||
                internalDepartments.Contains(s.InternalHospitalDepartmentType);
        }

        private static Expression<System.Func<SentinelEntry, bool>> LaboratoryNumberPredicate(SentinelEntrySearchTerm parsed)
        {
            var exactYear = parsed.ExactYear;
            var exactSeq = parsed.ExactSequenceNumber;
            var candidateYear = parsed.CandidateYear;
            var candidateSeq = parsed.CandidateSequenceNumber;

            return s =>
                (exactYear.HasValue && exactSeq.HasValue
                    && s.Year == exactYear.Value
                    && s.YearlySequentialEntryNumber == exactSeq.Value) ||
                (candidateYear.HasValue && s.Year == candidateYear.Value) ||
                (candidateSeq.HasValue && s.YearlySequentialEntryNumber == candidateSeq.Value);
        }

        private static Expression<System.Func<SentinelEntry, bool>> Or(
            Expression<System.Func<SentinelEntry, bool>> left,
            Expression<System.Func<SentinelEntry, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(SentinelEntry), "s");
            var leftBody = new ParameterReplacer(left.Parameters[0], parameter).Visit(left.Body);
            var rightBody = new ParameterReplacer(right.Parameters[0], parameter).Visit(right.Body);
            return Expression.Lambda<System.Func<SentinelEntry, bool>>(
                Expression.OrElse(leftBody!, rightBody!), parameter);
        }

        private sealed class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _source;
            private readonly ParameterExpression _target;

            public ParameterReplacer(ParameterExpression source, ParameterExpression target)
            {
                _source = source;
                _target = target;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _source ? _target : base.VisitParameter(node);
            }
        }
    }
}
