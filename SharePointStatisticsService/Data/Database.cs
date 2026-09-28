using Microsoft.Data.SqlClient;

namespace SharePointStatisticsService.Data
{
    public class Database(IConfiguration configuration)
    {
        public async Task TestUsageConnectionAsync(
            CancellationToken cancellationToken)
        {
            string? connectionString =
                configuration.GetConnectionString("UsageDatabase");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'UsageDatabase' is missing.");
            }

            await using SqlConnection connection =
                new(connectionString);

            await connection.OpenAsync(cancellationToken);

            await using SqlCommand command =
                new("SELECT 1", connection);

            await command.ExecuteScalarAsync(cancellationToken);
        }
    }
}