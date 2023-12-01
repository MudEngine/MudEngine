using System.Diagnostics.CodeAnalysis;
namespace MudEngine.CommandServer.Compiler;

public class CompilationResult
{
    public byte[]? AssemblyBytes { get; set; }
    public string? AssemblyCode { get; set; }
    public Guid CommandId { get; set; }
    public bool CompileError { get; set; }
    public bool NoCode { get; set; }
    public bool Success { get; set; }
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public List<string> CompileErrors { get; } = new();
}