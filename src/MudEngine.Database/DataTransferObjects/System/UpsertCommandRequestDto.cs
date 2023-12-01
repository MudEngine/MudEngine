using System.Diagnostics.CodeAnalysis;
namespace MudEngine.Database.DataTransferObjects.System;

public class UpsertCommandRequestDto
{
    public Guid CommandId { get; set; }
    public bool Preload { get; set; }
    public string Code { get; set; } = null!;
    [SuppressMessage("Style", "IDE0301:Simplify collection initialization", Justification = "Pending Resharper")]
    public byte[] AssemblyBinary { get; set; } = Array.Empty<byte>();
}