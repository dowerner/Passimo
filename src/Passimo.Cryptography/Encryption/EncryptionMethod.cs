namespace Passimo.Cryptography.Encryption;

public static class EncryptionMethod
{
    public static IEncryptionProcedure Aes128 => GetMethod(EncryptionType.Aes128);
    public static IEncryptionProcedure Aes256 => GetMethod(EncryptionType.Aes256);

    private static readonly Dictionary<EncryptionType, IEncryptionProcedure> _procedures = [];

    private static IEncryptionProcedure GetMethod(EncryptionType encryptionType)
    {
        if (!_procedures.ContainsKey(encryptionType))
        {
            _procedures[encryptionType] = EncryptionBuilder.Build(encryptionType);
        }
        return _procedures[encryptionType];
    }
}
