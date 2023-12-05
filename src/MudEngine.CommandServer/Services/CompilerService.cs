using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using MudEngine.CommandServer.Compiler;
using MudEngine.Library.System;
using IDatabaseRepository = MudEngine.Library.Interfaces.IDatabaseRepository;
namespace MudEngine.CommandServer.Services;

public class CompilerService
{
    private readonly string _dotNetRuntimePath;
    private readonly List<MetadataReference> _metadataReferences;
    private readonly string _usings;
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public CompilerService()
    {
        _dotNetRuntimePath = Path.GetDirectoryName(typeof(Console).Assembly.Location)!;
        _metadataReferences = new List<MetadataReference>
            {MetadataReference.CreateFromFile(typeof(ICommand).Assembly.Location)};
        if (string.IsNullOrWhiteSpace(_dotNetRuntimePath))
        {
            _usings = string.Empty;
            return;
        }
        _metadataReferences.Add(MetadataReference.CreateFromFile(typeof(IDatabaseRepository).Assembly.Location));
        var usings = new StringBuilder($"using {typeof(ICommand).Namespace};\r\n");
        usings.AppendLine("using MudEngine.Database.Interfaces;");
        AddMetadataReferences(usings,
            new List<string> {"Microsoft.CSharp.dll", "System.Private.CoreLib.dll", "System.Runtime.dll"},
            new List<string> {"using System;", "using System.Text;"});
        AddMetadataReferences(usings,
            new List<string> {"System.Collections.dll"},
            new List<string> {"using System.Collections;", "using System.Collections.Generic;"});
        AddMetadataReferences(usings,
            new List<string> {"System.Globalization.dll", "System.Globalization.Extensions.dll"},
            new List<string> {"using System.Globalization;"});
        AddMetadataReferences(usings,
            new List<string> {"System.Linq.dll"},
            new List<string> {"using System.Linq;"});
        AddMetadataReferences(usings,
            new List<string> {"System.Linq.Expressions.dll"},
            new List<string> {"using System.Linq.Expressions;"});
        AddMetadataReferences(usings,
            new List<string> {"System.Text.Json.dll"},
            new List<string> {"using System.Text.Json;"});
        AddMetadataReferences(usings,
            new List<string> {"System.Text.RegularExpressions.dll"},
            new List<string> {"using System.Text.RegularExpressions;"});
        AddMetadataReferences(usings,
            new List<string> {"System.ValueTuple.dll"},
            new List<string>());
        _usings = usings.ToString();
    }
    private void AddMetadataReferences(StringBuilder usings, IEnumerable<string> assembliesToInclude,
        IEnumerable<string> associatedHeaders)
    {
        foreach (var assemblyFile in assembliesToInclude
                     .Select(assemblyToInclude => assemblyToInclude.Contains('\\')
                         ? assemblyToInclude
                         : Path.Combine(_dotNetRuntimePath, assemblyToInclude))
                     .Where(File.Exists))
        {
            _metadataReferences.Add(MetadataReference.CreateFromFile(assemblyFile));
        }
        foreach (var associatedHeader in associatedHeaders)
        {
            if (!string.IsNullOrWhiteSpace(associatedHeader))
            {
                usings.AppendLine(associatedHeader);
            }
        }
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    public CompilationResult CompileCommand(string className, string code)
    {
        var compilationResult = new CompilationResult();
        if (string.IsNullOrWhiteSpace(code))
        {
            compilationResult.NoCode = true;
            return compilationResult;
        }

        var commandId = Guid.NewGuid();

        var codeBuilder = new StringBuilder(_usings);
        codeBuilder.AppendLine("namespace MudEngine.Library.Commands.Generated;");
        codeBuilder.AppendLine($"[Command(\"{commandId:D}\")]");
        codeBuilder.AppendLine($"public class {className} : BaseCommand, ICommand");
        codeBuilder.AppendLine("{");
        codeBuilder.AppendLine(
            "    public override CommandResponse Execute(CommandRequest Request)");
        codeBuilder.AppendLine("    {");
        codeBuilder.AppendLine("    base.Execute(Request);");
        codeBuilder.AppendLine(code);
        codeBuilder.AppendLine("        return Response;");
        codeBuilder.AppendLine("    }");
        codeBuilder.Append('}');

        var sourceText = SourceText.From(codeBuilder.ToString());
        compilationResult.AssemblyCode = new StringBuilder()
            .AppendJoin("\r\n", sourceText.Lines)
            .ToString();

        var compiler = CSharpCompilation.Create($"{commandId:N}",
            new[]
            {
                SyntaxFactory.ParseSyntaxTree(
                    sourceText,
                    CSharpParseOptions
                        .Default
                        .WithLanguageVersion(LanguageVersion.LatestMajor))
            },
            _metadataReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

        using var assemblyStream = new MemoryStream();
        var compilerResult = compiler.Emit(assemblyStream);

        if (!compilerResult.Success)
        {
            compilationResult.CompileError = true;
            foreach (var diagnostic in compilerResult.Diagnostics)
            {
                compilationResult.CompileErrors.Add(diagnostic.ToString());
            }
            return compilationResult;
        }

        assemblyStream.Seek(0, SeekOrigin.Begin);
        var assemblyBytes = assemblyStream.ToArray();
        compilationResult.Success = true;
        compilationResult.CommandId = commandId;
        compilationResult.AssemblyBytes = assemblyBytes;
        return compilationResult;
    }
}