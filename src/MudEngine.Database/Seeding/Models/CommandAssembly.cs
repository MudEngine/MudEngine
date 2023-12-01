using System;
using System.Collections.Generic;

namespace MudEngine.Database.Seeding.Models;

public partial class CommandAssembly
{
    public Guid CommandAssemblyId { get; set; }

    public bool Preload { get; set; }

    public string? SourceCode { get; set; }

    public byte[]? Binary { get; set; }
}
