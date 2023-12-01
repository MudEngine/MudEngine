using MudEngine.CommandServer.Services;
namespace MudEngine.CommandServer.Configuration.Host;

public class ServiceHost : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly CommandService _commandService;
    public ServiceHost(IHostApplicationLifetime appLifetime,
        CommandService commandService)
    {
        _hostApplicationLifetime = appLifetime;
        _commandService = commandService;
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
                    await _commandService.Start(stoppingToken).ConfigureAwait(false);
                    stoppingToken.ThrowIfCancellationRequested();
                    await Task.Delay(5000, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }, stoppingToken);
    }
    private void OnStopping()
    {
        _commandService.Stop();
    }
}