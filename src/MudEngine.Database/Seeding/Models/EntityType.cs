using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class EntityType
{
    public int EntityTypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Entity> Entities { get; set; } = new List<Entity>();
}
