using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class ConnectionCommand
{
    public Guid ConnectionId { get; set; }

    public int CommandListId { get; set; }
}
