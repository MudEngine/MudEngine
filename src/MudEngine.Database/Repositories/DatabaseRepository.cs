using System.Data;
using System.Diagnostics.CodeAnalysis;
using MudEngine.Database.Interfaces;
namespace MudEngine.Database.Repositories;

[SuppressMessage("ReSharper.DPA", "DPA0005: Database issues")]
public partial class DatabaseRepository(ILogger<DatabaseRepository> logger,
        Func<IDbConnection> connectionFactory)
    : IDatabaseRepository
{
}