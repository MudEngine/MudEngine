using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class Entity
{
    public int EntityId { get; set; }

    public int EntityTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? ParentEntityId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime LastUpdatedOn { get; set; }

    public bool InActive { get; set; }

    public virtual Clothing? Clothing { get; set; }

    public virtual Container? Container { get; set; }

    public virtual EntityType EntityType { get; set; } = null!;

    public virtual Living? Living { get; set; }

    public virtual Mobile? Mobile { get; set; }

    public virtual Mud? Mud { get; set; }

    public virtual Player? Player { get; set; }

    public virtual Room? Room { get; set; }

    public virtual Weapon? Weapon { get; set; }

    public virtual Zone? Zone { get; set; }
}
