using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class PlayerAlias
{
    public int PlayerId { get; set; }

    public string Alias { get; set; } = null!;

    public string Replacement { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
