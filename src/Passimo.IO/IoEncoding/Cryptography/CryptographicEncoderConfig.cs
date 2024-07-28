using Passimo.Cryptography;

namespace Passimo.IO.IoEncoding.Cryptography;

public class CryptographicEncoderConfig
{
    public int ChunkSize { get; init; }
    public int Parallelism { get; init; }
    public EncryptionType DefaultEncryptionType { get; init; }

    public static CryptographicEncoderConfig Default() => new()
    {
        ChunkSize = 1048576,
        Parallelism = Environment.ProcessorCount,
        DefaultEncryptionType = EncryptionType.Aes256
    };
}
