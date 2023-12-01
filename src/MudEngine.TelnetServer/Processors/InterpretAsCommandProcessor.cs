using System.Diagnostics.CodeAnalysis;
using MudEngine.TelnetServer.DataTransferObjects;
namespace MudEngine.TelnetServer.Processors;

public class InterpretAsCommandProcessor
{
    private const byte _iacse = 240;
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Pending Resharper")]
    public InterpretAsCommandDto Process(int iacPosition, byte[] buffer)
    {
        var iacResponse = new InterpretAsCommandDto
        {
            Buffer = buffer
        };
        var controlCode = iacResponse.Buffer[iacPosition + 1];
        byte[] iac;
        switch (controlCode)
        {
            case 250:
                var terminatorId = Array.IndexOf(iacResponse.Buffer, _iacse, iacPosition);
                if (terminatorId < 0)
                {
                    return iacResponse;
                }
                iac = new byte[terminatorId - iacPosition + 1];
                iacResponse.Buffer = Remove(iacResponse.Buffer, iacPosition, ref iac);
                break;
            case 251:
            case 252:
            case 253:
            case 254:
                if (iacResponse.Buffer.Length > iacPosition + 2)
                {
                    iac = new byte[3];
                    iacResponse.Buffer = Remove(iacResponse.Buffer, iacPosition, ref iac);
                }
                else
                {
                    return iacResponse;
                }
                break;
            default:
                return iacResponse;
        }
        switch (controlCode)
        {
            // NEGOTIATE SUBOPTIONS
            case 250:
            {
                var subOptionCode = iac[2];
                if (subOptionCode != 201) // WE ONLY HANDLE GMCP RIGHT NOW
                {
                    return iacResponse;
                }
                var gmcp = new byte[iac.Length - 5];
                Buffer.BlockCopy(iac, 3, gmcp, 0, iac.Length - 5);
                iacResponse.GenericMudCommunicationProtocol.Add(gmcp);
                break;
            }
            // WILL
            case 251:
            {
                if (iac[2] == 201 || iac[2] == 70) // GMCP or MSSP
                {
                    iacResponse.Commands.Add(new byte[] {255, 251, iac[2]}); // OK
                }
                break;
            }
            // WONT
            case 252:
                iacResponse.Commands.Add(new byte[] {255, 254, iac[2]}); // YOU WILL
                break;
            // DO
            case 253:
                switch (iac[2])
                {
                    case 3: // SUPPRESS GOAHEAD
                        iacResponse.Commands.Add(new byte[] {255, 251, 3}); // OK
                        break;
                    case 70: // Provide MSSP
                        iacResponse.ProvideServerStatus = true;
                        break;
                    case 201: // ACCEPT GMCP
                        break; // OK
                    default: // OTHERWISE
                        iacResponse.Commands.Add(new byte[] {255, 252, iac[2]}); // WONT
                        break;
                }
                break;
            // DONT
            default:
            {
                if (iac[2] == 3) // SUPPRESS GOAHEAD
                {
                    iacResponse.Commands.Add(new byte[] {255, 253, 3}); // DO
                }
                break;
            }
        }
        return iacResponse;
    }
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Injected")]
    [SuppressMessage("Style", "IDE0301:Simplify collection initialization", Justification = "Pending Resharper")]
    private byte[] Remove(byte[] data, int start, ref byte[] buffer)
    {
        Buffer.BlockCopy(data, start, buffer, 0, buffer.Length);
        if (data.Length == buffer.Length)
        {
            return Array.Empty<byte>();
        }
        var newArray = new byte[data.Length - buffer.Length];
        if (start > 0)
        {
            Buffer.BlockCopy(data, 0, newArray, 0, start);
            Buffer.BlockCopy(data, start + buffer.Length, newArray, start, newArray.Length - start);
        }
        else
        {
            Buffer.BlockCopy(data, buffer.Length, newArray, 0, data.Length - buffer.Length);
        }
        return newArray;
    }
}