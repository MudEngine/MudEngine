using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using MudEngine.Library.Interfaces;
namespace MudEngine.Library.Repositories;

[SuppressMessage("ReSharper.DPA", "DPA0005: Database issues")]
public partial class DatabaseRepository(ILogger<DatabaseRepository> logger,
        Func<IDbConnection> connectionFactory)
    : IDatabaseRepository
{
}