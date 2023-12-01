using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MudEngine.CommandServer.Compiler;
using MudEngine.CommandServer.Services;
using MudEngine.Database.Interfaces;
using MudEngine.Library.System;
using NUnit.Framework;
namespace MudEngine.Tests.IntegrationTests;

public class CompilationTests
{
    private CollectibleAssemblyLoadContext? _collectibleAssemblyLoadContext;
    private CompilerService? _compilerService;
    private IDatabaseRepository? _databaseRepository;
    [Test]
    public async Task CompilerTest()
    {
        var result = _compilerService?.CompileCommand("CompilerTest",
                         "AddMessage(\"CompilerTest: \" " +
                         "+ Request.CommandLine);")
                     ?? new CompilationResult();
        var command = _collectibleAssemblyLoadContext?
            .GetCommandsFromCollectibleAssembly(result.AssemblyBytes!)
            .FirstOrDefault().Value;
        var cancellationTokenSource = new CancellationTokenSource();
        var connectionId = Guid.NewGuid();
        var commandResponse = command!.Execute(
            new CommandRequest(_databaseRepository!, connectionId, connectionId.ToString("N"), cancellationTokenSource.Token));
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(commandResponse?.ResponseMessages.FirstOrDefault()?.Text.ToString() 
                        ?? string.Empty,
                Does.Contain(connectionId.ToString("N")));
        });
    }
    [OneTimeSetUp]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        var factory = serviceProvider.GetService<ILoggerFactory>();
        var logger = factory!.CreateLogger<CollectibleAssemblyLoadContext>();
        _databaseRepository = new Mock<IDatabaseRepository>().Object;
        _compilerService = new CompilerService();
        _collectibleAssemblyLoadContext = new CollectibleAssemblyLoadContext(logger);
    }
}