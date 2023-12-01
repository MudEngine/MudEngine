using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using MudEngine.Library.System;
namespace MudEngine.CommandServer.Compiler;

public class CollectibleAssemblyLoadContext(ILogger<CollectibleAssemblyLoadContext> logger) : AssemblyLoadContext(true)
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public Dictionary<Guid, ICommand> GetCommandsFromCollectibleAssembly(byte[] assemblyBytes)
    {
        var commands = new Dictionary<Guid, ICommand>();
        if (assemblyBytes.Length == 0)
        {
            return commands;
        }
        try
        {
            using var assemblyBinary = new MemoryStream(assemblyBytes);
            assemblyBinary.Seek(0, SeekOrigin.Begin);
            var assembly = LoadFromStream(assemblyBinary);
            foreach (var type in assembly
                         .GetTypes()
                         .Where(m =>
                             m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0
                             && m.GetInterfaces().Any(i => i is {IsPublic: true, Name: "ICommand"}))
                         .ToList())
            {
                var commandId = ((CommandAttribute) Attribute.GetCustomAttribute(type, typeof(CommandAttribute))!)
                    .CommandId;
                commands.Add(commandId, (Activator.CreateInstance(type) as ICommand)!);
            }
        }
        catch (Exception e)
        {
            logger.LogInformation(e, "GetCommandsFromCollectibleAssembly");
        }
        return commands;
    }
    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null!;
    }
}