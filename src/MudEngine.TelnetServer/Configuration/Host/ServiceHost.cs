using MudEngine.TelnetServer.Services;
namespace MudEngine.TelnetServer.Configuration.Host;

public class ServiceHost : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<ServiceHost> _logger;
    private readonly TelnetService _telnetService;
    public ServiceHost(IHostApplicationLifetime appLifetime,
        ILogger<ServiceHost> logger,
        TelnetService telnetService)
    {
        _hostApplicationLifetime = appLifetime;
        _logger = logger;
        _telnetService = telnetService;
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
                    await _telnetService.Start(stoppingToken).ConfigureAwait(false);
                    stoppingToken.ThrowIfCancellationRequested();
                    await Task.Delay(5000, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception e)
            {
                _logger.LogTrace(e, "TelnetServer_ServiceHost_ExecuteAsync");
            }
            finally
            {
                _logger.LogInformation("Telnet stopped at: {time}", DateTimeOffset.Now);
                _hostApplicationLifetime.StopApplication();
            }
        }, stoppingToken);
    }
    private void OnStopping()
    {
        _telnetService.Stop();
    }
}