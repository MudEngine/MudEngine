namespace MudEngine.HubServer.Services.HubHandling;

public struct ThreadLock : IDisposable
{
    private object _syncObject;
    private bool _lockTaken;
    private const int TimeoutMaxWait = 100;
    public bool Lock(object onObject)
    {
        _syncObject = onObject;
        Monitor.TryEnter(onObject, TimeoutMaxWait, ref _lockTaken);
        return _lockTaken;
    }
    public void Dispose()
    {
        if (!_lockTaken)
        {
            return;
        }
        _lockTaken = false;
        Monitor.Exit(_syncObject);
    }
}