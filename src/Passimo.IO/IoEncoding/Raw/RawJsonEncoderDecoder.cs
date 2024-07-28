using Passimo.IO.Exceptions;
using System.Text.Json;

namespace Passimo.IO.IoEncoding.Raw;

public class RawJsonEncoderDecoder<T> : IRawEncoderDecoder<T> where T : class
{
    public async Task<T> DecodeAsync(byte[] buffer, CancellationToken ct = default)
    {
        await using var stream = new MemoryStream(buffer);
        return await DecodeAsync(stream, ct);
    }

    public async Task<T> DecodeAsync(System.IO.Stream stream, CancellationToken ct = default)
    {
        var result = await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: ct);
        return result ?? throw new PassimoEncodingException("Unable to decode stream");
    }

    public async Task<byte[]> EncodeAsync(T value, CancellationToken ct = default)
    {
        await using var stream = new MemoryStream();
        await EncodeAsync(stream, value, ct);
        return stream.ToArray();
    }

    public async Task EncodeAsync(System.IO.Stream stream, T value, CancellationToken ct = default)
    {
        await JsonSerializer.SerializeAsync(stream, value, cancellationToken: ct);
    }
}
