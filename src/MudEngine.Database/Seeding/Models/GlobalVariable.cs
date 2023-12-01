using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class GlobalVariable
{
    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;
}
