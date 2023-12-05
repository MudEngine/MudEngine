using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class Mud
{
    public int EntityId { get; set; }

    public Guid OnNewConnectionCommandId { get; set; }

    public string? LoginScreen { get; set; }

    public string? News { get; set; }

    public virtual Entity Entity { get; set; } = null!;
}
