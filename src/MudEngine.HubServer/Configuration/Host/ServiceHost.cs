using MudEngine.HubServer.Services;
namespace MudEngine.HubServer.Configuration.Host;

public class ServiceHost : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly HubService _hubService;
    private readonly ILogger<ServiceHost> _logger;
    public ServiceHost(IHostApplicationLifetime appLifetime,
        ILogger<ServiceHost> logger,
        HubService hubService)
    {
        _hostApplicationLifetime = appLifetime;
        _logger = logger;
        _hubService = hubService;
        appLifetime.ApplicationStopping.Register(OnStopping);
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    await _hubService.Start(stoppingToken).ConfigureAwait(false);
                    stoppingToken.ThrowIfCancellationRequested();
                    await Task.Delay(5000, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception e)
            {
                _logger.LogTrace(e, "HubServer_ServiceHost_ExecuteAsync");
            }
            finally
            {
                _logger.LogWarning("Hub stopped at: {time}", DateTimeOffset.Now);
                _hostApplicationLifetime.StopApplication();
            }
        }, stoppingToken);
    }
    private void OnStopping()
    {
        _hubService.Stop();
    }
}