using SharePointStatisticsService.Collectors;
using SharePointStatisticsService.Data;
using SharePointStatisticsService.Models;

namespace SharePointStatisticsService
{
    public class Worker(
        ILogger<Worker> logger,
        IConfiguration configuration,
        Database database,
        UsageCollector usageCollector) : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            int intervalSeconds = configuration.GetValue<int>(
                "Statistics:IntervalSeconds");

            if (intervalSeconds <= 0)
            {
                throw new InvalidOperationException(
                    "Statistics:IntervalSeconds must be greater than 0.");
            }

            logger.LogInformation(
                "SharePoint Statistics Service started. Interval: {intervalSeconds} seconds.",
                intervalSeconds);

            logger.LogInformation(
                "Testing connection to Usage database.");

            await database.TestUsageConnectionAsync(stoppingToken);

            logger.LogInformation(
                "Connection to Usage database successful.");

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Usage collection started.");

                List<UsageEvent> usageEvents =
                    await usageCollector.ReadTestDataAsync(stoppingToken);

                logger.LogInformation(
                    "Usage collection finished. Events read: {eventCount}.",
                    usageEvents.Count);

                await Task.Delay(
                    TimeSpan.FromSeconds(intervalSeconds),
                    stoppingToken);
            }
        }
    }
}