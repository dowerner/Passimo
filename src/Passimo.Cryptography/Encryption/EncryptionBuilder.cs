namespace Passimo.Cryptography.Encryption;
internal static class EncryptionBuilder
{
    public static IEncryptionProcedure Build(EncryptionType encryptionType)
    {
        return encryptionType switch
        {
            EncryptionType.Aes128 => new EncryptionProcedureAESThenHMAC(128, 64, 10000, 5),
            EncryptionType.Aes256 => new EncryptionProcedureAESThenHMAC(256, 64, 10000, 5),
            _ => throw new ArgumentException($"Unsupported encryption type '{encryptionType}'", nameof(encryptionType))
        };
    }
}
