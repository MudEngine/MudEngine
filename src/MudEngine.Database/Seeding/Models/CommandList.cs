using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class CommandList
{
    public int CommandListId { get; set; }

    public string Name { get; set; } = null!;

    public int Priority { get; set; }
}
