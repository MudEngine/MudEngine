using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using MudEngine.TelnetServer.DataTransferObjects;
namespace MudEngine.TelnetServer.Processors;

public class TelnetDataProcessor(ILogger<TelnetDataProcessor> logger,
    InterpretAsCommandProcessor interpretAsCommandProcessor)
{
    private const byte _iac = 255;
    private const byte _cr = 13;
    private const byte _lf = 10;
    private readonly Pipe _unprocessedData = new();
    [SuppressMessage("Style", "IDE0301:Simplify collection initialization", Justification = "Pending Resharper")]
    private async Task<TelnetDataDto> ProcessData(byte[] data, bool hasControlCode, bool hasLine,
        CancellationToken token)
    {
        var telnetDataProcessorResponse = new TelnetDataDto();
        if (hasControlCode)
        {
            var iacPosition = Array.IndexOf(data, _iac);
            while (iacPosition >= 0)
            {
                if (data.Length <= iacPosition + 1)
                {
                    return telnetDataProcessorResponse;
                }
                var interpretAsCommandResponse = interpretAsCommandProcessor.Process(iacPosition, data);
                data = interpretAsCommandResponse.Buffer;
                telnetDataProcessorResponse.GMCPRequests.AddRange(interpretAsCommandResponse
                    .GenericMudCommunicationProtocol);
                telnetDataProcessorResponse.Commands.AddRange(interpretAsCommandResponse.Commands);
                telnetDataProcessorResponse.ProvideServerStatus = interpretAsCommandResponse.ProvideServerStatus;
                iacPosition = Array.IndexOf(data, _iac);
            }
        }
        if (hasLine && data.Length > 0)
        {
            var lfStart = Array.IndexOf(data, _lf);
            var crStart = Array.IndexOf(data, _cr);
            while (lfStart >= 0 || crStart >= 0)
            {
                var crpos = lfStart > crStart ? lfStart : crStart;
                var line = new byte[crpos + 1];
                Buffer.BlockCopy(data, 0, line, 0, line.Length);
                if (data.Length == line.Length)
                {
                    data = Array.Empty<byte>();
                }
                else
                {
                    var remainingBytes = new byte[data.Length - line.Length];
                    Buffer.BlockCopy(data, line.Length, remainingBytes, 0, data.Length - line.Length);
                    data = remainingBytes;
                }
                telnetDataProcessorResponse.Lines.Add(line);
                lfStart = Array.IndexOf(data, _lf);
                crStart = Array.IndexOf(data, _cr);
            }
        }
        if (data.Length > 0)
        {
            await _unprocessedData.Writer.WriteAsync(data, token).ConfigureAwait(false);
        }
        return telnetDataProcessorResponse;
    }
    public async Task<TelnetDataDto> ProcessSegment(ReadOnlyMemory<byte> segment, CancellationToken token)
    {
        try
        {
            await _unprocessedData.Writer.WriteAsync(segment, token).ConfigureAwait(false);
            var fileData = await _unprocessedData.Reader.ReadAsync(token).ConfigureAwait(false);
            var buffer = fileData.Buffer;
            if (buffer.Length > 4000)
            {
                logger.LogInformation("Exceeded DATA buffer size constraint: [{data}]", buffer.Length.ToString());
                await _unprocessedData.Reader.CompleteAsync().ConfigureAwait(false);
                await _unprocessedData.Writer.CompleteAsync().ConfigureAwait(false);
                _unprocessedData.Reset();
                return new TelnetDataDto();
            }
            var hasControlCode = buffer.PositionOf(_iac) != null;
            var hasLine = (buffer.PositionOf(_lf) ?? buffer.PositionOf(_cr)) != null;
            if (hasControlCode || hasLine)
            {
                var data = buffer.ToArray();
                _unprocessedData.Reader.AdvanceTo(buffer.End);
                return await ProcessData(data, hasControlCode, hasLine, token).ConfigureAwait(false);
            }
            _unprocessedData.Reader.AdvanceTo(buffer.Start);
        }
        catch (Exception e)
        {
            logger.LogError(e, "ProcessSegment");
        }
        return new TelnetDataDto();
    }
}