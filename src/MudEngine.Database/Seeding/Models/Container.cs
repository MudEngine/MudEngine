using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class Container
{
    public int EntityId { get; set; }

    public int ContainerTypeId { get; set; }

    public int Capacity { get; set; }

    public virtual ContainerType ContainerType { get; set; } = null!;

    public virtual Entity Entity { get; set; } = null!;
}
