using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class Player
{
    public int EntityId { get; set; }

    public Guid AccountId { get; set; }

    public virtual Entity Entity { get; set; } = null!;

    public virtual ICollection<PlayerAlias> PlayerAliases { get; set; } = new List<PlayerAlias>();
}
