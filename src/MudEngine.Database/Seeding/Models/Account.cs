using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class Account
{
    public Guid AccountId { get; set; }

    public string Name { get; set; } = null!;

    public string? HashedPassword { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? LastAccessed { get; set; }
}
