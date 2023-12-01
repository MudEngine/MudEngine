using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class ConnectionVariable
{
    public Guid ConnectionId { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;
}
