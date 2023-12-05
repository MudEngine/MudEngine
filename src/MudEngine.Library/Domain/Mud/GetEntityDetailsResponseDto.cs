namespace MudEngine.Library.Domain.Mud;

public class GetEntityDetailsResponseDto : GetEntityDetailsEntityResponseDto
{
    public string? Description { get; set; }
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Pending Resharper")]
    public List<GetEntityDetailsEntityResponseDto> Entities { get; } = new();
}