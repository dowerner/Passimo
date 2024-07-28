using Passimo.Cryptography;

namespace Passimo.IO.IoEncoding.FileStream.Headers;

public class ChunkHeader : HeaderBase
{
    public EncryptionType EncryptionType { get; set; }
    public uint ChunkLength { get; set; }

    protected override void DefineHeader()
    {
        DefineUshort16(() => (ushort)EncryptionType, e => EncryptionType = (EncryptionType)e);
        DefineUint32(() => ChunkLength, l => ChunkLength = l);
    }
}
