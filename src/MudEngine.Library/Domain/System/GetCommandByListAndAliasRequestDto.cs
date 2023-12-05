namespace MudEngine.Library.Domain.System;

public class GetCommandByListAndAliasRequestDto
{
    public string? CommandListName { get; set; }
    public string? PrimaryAlias { get; set; }
}