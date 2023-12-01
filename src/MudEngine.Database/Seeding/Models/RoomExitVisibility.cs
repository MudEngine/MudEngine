using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class RoomExitVisibility
{
    public int RoomExitVisibilityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<RoomExit> RoomExits { get; set; } = new List<RoomExit>();
}
