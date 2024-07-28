using Passimo.Cryptography;

namespace Passimo.IO.IoEncoding.FileStream;

public class PassimoFileEncryptedChunk
{
    public EncryptionType EncryptionType { get; set; }
    public byte[] EncryptedBuffer { get; set; } = null!;
}

public class PassimoFile
{
    public ushort FileVersion { get; set; } = 1;
    public PassimoFileEncryptedChunk[] EncryptedChunks { get; set; } = null!;
}
