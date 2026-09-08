namespace Aggregation.Backend.Domain.Constants
{
    public static class Policies
    {
        public const string AggregatesCachePolicy = nameof(AggregatesCachePolicy);
        public const string KeywordQueryParam = "keyword";
        public const string FilterQueryParam = "filterBy";
        public const string SortQueryParam = "orderBy";
        public const string SortTypeQueryParam = "asc";
    }
}