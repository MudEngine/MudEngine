using System.Text;
using System.Text.Json;
using MudEngine.TelnetServer.DataTransferObjects;
namespace MudEngine.TelnetServer.Processors;

public static class MsspProcessor
{
    public static byte[] ToMssp(this string msspJson)
    {
        var msspData =
            JsonSerializer.Deserialize<IEnumerable<SystemMsspResponseDto>>(msspJson);
        IEnumerable<byte> mssp = new byte[] {255, 250, 70};
        mssp = msspData!.Aggregate(mssp, (current, msspEntry) =>
            current.Concat(new byte[] {1})
                .Concat(Encoding.UTF8.GetBytes(msspEntry.Key!))
                .Concat(new byte[] {2})
                .Concat(Encoding.UTF8.GetBytes(msspEntry.Value!)));
        mssp = mssp.Concat(new byte[] {255, 240});
        return mssp.ToArray();
    }
}