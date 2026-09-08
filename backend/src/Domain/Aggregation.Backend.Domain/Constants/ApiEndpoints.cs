namespace Aggregation.Backend.Domain.Constants
{
    public static class ApiEndpoints
    {
        public const string Api = "api";
        public const string Version = "1";
        public const string Aggregates = "aggregates";
        public const string Statistics = "statistics";
        public const string GetAggregatesRoute = $"{Api}/v{Version}/{Aggregates}";
        public const string GetStatisticsRoute = $"{Api}/v{Version}/{Statistics}";
        public const string Authenticate = $"{Api}/v{Version}/auth";

    }
}