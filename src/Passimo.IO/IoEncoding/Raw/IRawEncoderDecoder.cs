namespace Passimo.IO.IoEncoding.Raw;

public interface IRawEncoderDecoder<T> where T : class
{
    Task<byte[]> EncodeAsync(T value, CancellationToken ct = default);
    Task EncodeAsync(System.IO.Stream stream, T value, CancellationToken ct = default);
    Task<T> DecodeAsync(byte[] buffer, CancellationToken ct = default);
    Task<T> DecodeAsync(System.IO.Stream buffer, CancellationToken ct = default);
}
