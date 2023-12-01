using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class RoomExit
{
    public int SourceId { get; set; }

    public int DestinationId { get; set; }

    public string PrimaryAlias { get; set; } = null!;

    public int RoomExitVisibilityId { get; set; }

    public virtual RoomExitVisibility RoomExitVisibility { get; set; } = null!;
}
