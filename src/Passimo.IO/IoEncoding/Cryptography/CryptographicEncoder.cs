using Passimo.Domain.Model;
using Passimo.IO.IoEncoding.Raw;
using Passimo.Cryptography.Encryption;
using Microsoft.Extensions.Options;
using Passimo.IO.IoEncoding.FileStream;
using System.Text;

namespace Passimo.IO.IoEncoding.Cryptography;

public class CryptographicEncoder(
    IRawEncoderDecoder<PasswordProfile> encoderDecoder,
    IFileStreamEncoderDecoder fileEncoderDecoder,
    IOptions<CryptographicEncoderConfig>? options = null) : ICryptographicEncoder
{
    private CryptographicEncoderConfig Config => _config ??= options?.Value ?? CryptographicEncoderConfig.Default();
    private CryptographicEncoderConfig? _config;

    public async Task EncrypPasswordProfileAsync(PasswordProfile passwordProfile, System.IO.Stream targetStream, string password, CancellationToken ct = default)
    {
        var buffer = await encoderDecoder.EncodeAsync(passwordProfile, ct);
        var sourceStream = new MemoryStream(buffer);
        await EncryptStreamAsync(sourceStream, targetStream, password, ct);
    }

    public async Task<System.IO.Stream> EncrypPasswordProfileAsync(PasswordProfile passwordProfile, string password, CancellationToken ct = default)
    {
        var targetStream = new MemoryStream();
        await EncrypPasswordProfileAsync(passwordProfile, targetStream, password, ct);
        return targetStream;
    }

    public async Task EncryptStreamAsync(System.IO.Stream sourceStream, System.IO.Stream targetStream, string password, CancellationToken ct = default)
    {
        // divide stream data into chunks
        var chunks = new List<byte[]>();
        while (sourceStream.Length - 1 > sourceStream.Position)
        {
            var sizeLeft = sourceStream.Length - sourceStream.Position;
            var chunkSize = (int) Math.Min(sizeLeft, Config.ChunkSize);
            var chunk = new byte[chunkSize];
            await sourceStream.ReadAsync(chunk, 0, chunkSize, ct);
            chunks.Add(chunk);
        }

        // encrypt the chunks in parallel
        var obj = new object();
        var encrypedChunks = new List<(int, byte[])>();
        var options = new ParallelOptions { CancellationToken = ct, MaxDegreeOfParallelism = Config.Parallelism };
        var encryptionProcedure = EncryptionMethod.GetMethod(Config.DefaultEncryptionType);
        Parallel.For(0, chunks.Count, options, chunkIndex =>
        {
            var chunk = chunks[chunkIndex];
            var encrypedChunk = encryptionProcedure.EncryptWithPassword(chunk, password);            
            lock (obj)
            {
                encrypedChunks.Add((chunkIndex, encrypedChunk));
            }
        });

        // order the chunks and append them to the target stream
        var resultChunks = encrypedChunks.OrderBy(c => c.Item1).Select(c => c.Item2).ToArray();
        var passimoFile = new PassimoFile
        {
            FileVersion = 1,
            EncryptedChunks = resultChunks.Select(c => new PassimoFileEncryptedChunk
            {
                EncryptionType = Config.DefaultEncryptionType,
                EncryptedBuffer = c,
            }).ToArray()
        };

        // write the file structure to stream
        await fileEncoderDecoder.WriteEncryptedChunksToStreamAsync(passimoFile, targetStream, ct);
    }

    public async Task<System.IO.Stream> EncryptStreamAsync(System.IO.Stream stream, string password, CancellationToken ct = default)
    {
        var targetStream = new MemoryStream();
        await EncryptStreamAsync(stream, targetStream, password, ct);
        return targetStream;
    }

    public async Task<PasswordProfile> DecryptPasswordProfileAsync(System.IO.Stream stream, string password, CancellationToken ct = default)
    {
        // get encrypted chunks from stream
        var passwordFile = await fileEncoderDecoder.ReadEncrypedChunksFromStreamAsync(stream, ct);

        // decrypt chunks in parallel
        var obj = new object();
        var decryptedChunks = new List<(int, byte[])>();
        Parallel.For(0, passwordFile.EncryptedChunks.Length, chunkIndex =>
        {
            var chunk = passwordFile.EncryptedChunks[chunkIndex];
            var encryptionMethod = EncryptionMethod.GetMethod(chunk.EncryptionType);
            var decryptedChunk = encryptionMethod.DecryptWithPassword(chunk.EncryptedBuffer, password);
            lock (obj)
            {
                decryptedChunks.Add((chunkIndex, decryptedChunk));
            }
        });

        // order decryped chunks
        var resultChunks = decryptedChunks.OrderBy(c => c.Item1).Select(c => c.Item2).ToArray();

        // write chunks to new stream
        var targetStream = new MemoryStream();
        foreach (var chunk in resultChunks)
        {
            await targetStream.WriteAsync(chunk, ct);
        }

        // deserialize the decryped stream
        targetStream.Seek(0, SeekOrigin.Begin);
        return await encoderDecoder.DecodeAsync(targetStream, ct);
    }
}
