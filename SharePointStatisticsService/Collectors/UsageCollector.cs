using Microsoft.Data.SqlClient;
using SharePointStatisticsService.Models;

namespace SharePointStatisticsService.Collectors
{
    public class UsageCollector(
        IConfiguration configuration,
        ILogger<UsageCollector> logger)
    {
        public async Task<List<UsageEvent>> ReadTestDataAsync(
            CancellationToken cancellationToken)
        {
            string? connectionString =
                configuration.GetConnectionString("UsageDatabase");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'UsageDatabase' is missing.");
            }

            const string sql = """
                SELECT TOP (10)
                    LogTime,
                    UserLogin,
                    SiteUrl,
                    WebUrl,
                    DocumentPath,
                    RequestType,
                    HttpStatus,
                    ReferrerUrl,
                    RowId
                FROM RequestUsage
                ORDER BY LogTime DESC;
                """;

            List<UsageEvent> usageEvents = new();

            await using SqlConnection connection =
                new(connectionString);

            await connection.OpenAsync(cancellationToken);

            await using SqlCommand command =
                new(sql, connection);

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                UsageEvent usageEvent = new()
                {
                    LogTime = reader.GetDateTime(
                        reader.GetOrdinal("LogTime")),

                    UserLogin = GetNullableString(
                        reader, "UserLogin"),

                    SiteUrl = GetNullableString(
                        reader, "SiteUrl"),

                    WebUrl = GetNullableString(
                        reader, "WebUrl"),

                    DocumentPath = GetNullableString(
                        reader, "DocumentPath"),

                    RequestType = GetNullableString(
                        reader, "RequestType"),

                    HttpStatus = GetNullableShort(
                        reader, "HttpStatus"),

                    ReferrerUrl = GetNullableString(
                        reader, "ReferrerUrl"),

                    RowId = reader.GetGuid(
                        reader.GetOrdinal("RowId"))
                };

                usageEvents.Add(usageEvent);
            }

            logger.LogInformation(
                "Usage test read completed. Rows read: {rowCount}.",
                usageEvents.Count);

            return usageEvents;
        }

        private static string? GetNullableString(
            SqlDataReader reader,
            string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            return reader.IsDBNull(ordinal)
                ? null
                : reader.GetString(ordinal);
        }

        private static short? GetNullableShort(
            SqlDataReader reader,
            string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            return reader.IsDBNull(ordinal)
                ? null
                : reader.GetInt16(ordinal);
        }
    }
}