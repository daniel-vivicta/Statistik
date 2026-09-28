using SharePointStatisticsService;
using SharePointStatisticsService.Collectors;
using SharePointStatisticsService.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<Database>();
builder.Services.AddSingleton<UsageCollector>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();