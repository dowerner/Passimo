namespace Passimo.IO.IoEncoding.FileStream;

public interface IFileStreamEncoderDecoder
{
    Task WriteEncryptedChunksToStreamAsync(PassimoFile file, System.IO.Stream stream, CancellationToken ct = default);
    Task<PassimoFile> ReadEncrypedChunksFromStreamAsync(System.IO.Stream stream, CancellationToken ct = default);
}
