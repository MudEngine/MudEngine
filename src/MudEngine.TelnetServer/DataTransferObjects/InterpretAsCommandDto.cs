using System.Diagnostics.CodeAnalysis;
namespace MudEngine.TelnetServer.DataTransferObjects;

public class InterpretAsCommandDto
{
    [SuppressMessage("Style", "IDE0301:Simplify collection initialization", Justification = "Pending Resharper")]
    public byte[] Buffer { get; set; } = Array.Empty<byte>();
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public List<byte[]> Commands { get; } = new();
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public List<byte[]> GenericMudCommunicationProtocol { get; } = new();
    public bool ProvideServerStatus { get; set; }
}