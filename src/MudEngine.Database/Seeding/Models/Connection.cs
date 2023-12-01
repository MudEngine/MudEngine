using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class Connection
{
    public Guid ConnectionId { get; set; }

    public Guid? AccountId { get; set; }

    public int? PlayerId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime LastCommandRequestedOn { get; set; }
}
