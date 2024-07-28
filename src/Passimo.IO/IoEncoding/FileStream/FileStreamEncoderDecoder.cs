using Passimo.IO.Exceptions;
using Passimo.IO.IoEncoding.FileStream.Headers;

namespace Passimo.IO.IoEncoding.FileStream;

public class FileStreamEncoderDecoder : IFileStreamEncoderDecoder
{
    private const string ControlString = "PASSIMO";

    public async Task WriteEncryptedChunksToStreamAsync(PassimoFile file, System.IO.Stream stream, CancellationToken ct = default)
    {
        var fileHeader = HeaderBase.Create<FileHeader>();
        fileHeader.FormatString = ControlString;
        fileHeader.FileVersion = file.FileVersion;
        await fileHeader.WriteToStream(stream, ct);
        
        foreach (var chunk in file.EncryptedChunks)
        {
            var chunkHeader = HeaderBase.Create<ChunkHeader>();
            chunkHeader.EncryptionType = chunk.EncryptionType;
            chunkHeader.ChunkLength = (uint) chunk.EncryptedBuffer.Length;
            await chunkHeader.WriteToStream(stream, ct);
            await stream.WriteAsync(chunk.EncryptedBuffer, ct);
        }
    }

    public async Task<PassimoFile> ReadEncrypedChunksFromStreamAsync(System.IO.Stream stream, CancellationToken ct = default)
    {
        var fileHeader = HeaderBase.Create<FileHeader>();
        await fileHeader.ReadFromStream(stream, ct);

        if (fileHeader.FormatString != ControlString)
        {
            throw new PassimoEncodingException("Foramt string of file is invalid.");
        }

        var encryptedChunks = new List<PassimoFileEncryptedChunk>();

        while (stream.Length - 1 > stream.Position)
        {
            var chunkHeader = HeaderBase.Create<ChunkHeader>();
            await chunkHeader.ReadFromStream(stream, ct);

            var encryptedBuffer = new byte[chunkHeader.ChunkLength];
            await stream.ReadAsync(encryptedBuffer, 0, encryptedBuffer.Length);

            encryptedChunks.Add(new PassimoFileEncryptedChunk
            {
                EncryptionType = chunkHeader.EncryptionType,
                EncryptedBuffer = encryptedBuffer
            });
        }

        return new PassimoFile
        {
            FileVersion = fileHeader.FileVersion,
            EncryptedChunks = encryptedChunks.ToArray()
        };
    }
}
