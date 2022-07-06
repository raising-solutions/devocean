using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Devocean.Core.Infrastructure.Services.Mapping;

public class AutomapperConfigurationBackgroundService : BackgroundService
{
    public IServiceProvider Services { get; }

    private readonly ILogger<AutomapperConfigurationBackgroundService> _logger;

    public AutomapperConfigurationBackgroundService(IServiceProvider services,
        ILogger<AutomapperConfigurationBackgroundService> logger)
    {
        Services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "AutomapperConfigurationService is running.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "AutomapperConfigurationService is working.");

        await using var scope = Services.CreateAsyncScope();
        var mapperConfiguration = scope.ServiceProvider.GetRequiredService<AutoMapper.IConfigurationProvider>();
        mapperConfiguration.CompileMappings();
        mapperConfiguration.AssertConfigurationIsValid();
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "AutomapperConfigurationService is stopping.");

        await base.StopAsync(stoppingToken);
    }
}