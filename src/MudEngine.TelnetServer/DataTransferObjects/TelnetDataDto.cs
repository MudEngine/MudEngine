using System.Diagnostics.CodeAnalysis;

namespace MudEngine.TelnetServer.DataTransferObjects;

public class TelnetDataDto
{
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public List<byte[]> Lines { get; } = new();
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public List<byte[]> Commands { get; } = new();
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public List<byte[]> GMCPRequests { get; } = new();
    public bool ProvideServerStatus { get; set; }

}