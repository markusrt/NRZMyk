namespace NRZMyk.Server.Controllers.SentinelEntries
{
    public class ListPagedSentinelEntryRequest : BaseRequest
    {
        /// <summary>
        /// Number of entries per page. Defaults to <see cref="DefaultPageSize"/> when not provided
        /// and is clamped to the range [1, <see cref="MaxPageSize"/>].
        /// </summary>
        public int PageSize { get; set; } = DefaultPageSize;

        /// <summary>
        /// Zero-based page index. Defaults to 0 when not provided and is clamped to be non-negative.
        /// </summary>
        public int PageIndex { get; set; }

        public string? SearchTerm { get; set; }
        public int? OrganizationId { get; set; }

        public const int DefaultPageSize = 25;
        public const int MaxPageSize = 1000;
    }
}
