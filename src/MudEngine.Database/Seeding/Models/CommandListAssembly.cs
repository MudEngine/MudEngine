using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class CommandListAssembly
{
    public int CommandListId { get; set; }

    public Guid CommandAssemblyId { get; set; }

    public string? PrimaryAlias { get; set; }

    public bool HandlesUnknown { get; set; }
}
