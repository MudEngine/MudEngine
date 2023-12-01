namespace MudEngine.TelnetServer.DataTransferObjects;

public class SystemMsspResponseDtoList(List<SystemMsspResponseDto> data) : List<SystemMsspResponseDto>
{
    public List<SystemMsspResponseDto> Data { get; set; } = data;
}