using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class ContainerType
{
    public int ContainerTypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Container> Containers { get; set; } = new List<Container>();
}
