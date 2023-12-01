using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class ClientMessageType
{
    public int ClientMessageTypeId { get; set; }

    public string Name { get; set; } = null!;
}
