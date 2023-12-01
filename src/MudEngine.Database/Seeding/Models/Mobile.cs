using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class Mobile
{
    public int EntityId { get; set; }

    public virtual Entity Entity { get; set; } = null!;
}
