/*
* The methods from this class are based on the work of James Tuley
* Slighty modified to support SecureString
*/

/* ORIGINAL HEADER:
 * This work (Modern Encryption of a String C#, by James Tuley), 
 * identified by James Tuley, is free of known copyright restrictions.
 * https://gist.github.com/4336842
 * http://creativecommons.org/publicdomain/mark/1.0/ 
 */

using Passimo.Cryptography.Extensions;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Passimo.Cryptography.Encryption;

internal class EncryptionProcedureAESThenHMAC : IEncryptionProcedure
{
    //Preconfigured Encryption Parameters
    public static readonly int BlockBitSize = 128;
    public int KeyBitSize { get; }

    //Preconfigured Password Key Derivation Parameters
    public int SaltBitSize { get; }
    public int Iterations { get; }
    public int MinPasswordLength { get; }

    public EncryptionProcedureAESThenHMAC(int keyBitSize = 256, int saltBitSize = 64, int iterations = 10000, int minPasswordLength = 12)
    {
        KeyBitSize = keyBitSize;
        SaltBitSize = saltBitSize;
        Iterations = iterations;
        MinPasswordLength = minPasswordLength;
    }

    /// <summary>
    /// Simple Encryption (AES) then Authentication (HMAC) for a UTF8 Message.
    /// </summary>
    /// <param name="secretMessage">The secret message.</param>
    /// <param name="cryptKey">The crypt key.</param>
    /// <param name="authKey">The auth key.</param>
    /// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
    /// <returns>
    /// Encrypted Message
    /// </returns>
    /// <exception cref="ArgumentException">Secret Message Required!;secretMessage</exception>
    /// <remarks>
    /// Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +  HMac-Tag(32)) * 1.33 Base64
    /// </remarks>
    public string Encrypt(string secretMessage, byte[] cryptKey, byte[] authKey, byte[]? nonSecretPayload = null)
    {
        if (string.IsNullOrEmpty(secretMessage))
            throw new ArgumentException("Secret Message Required!", nameof(secretMessage));

        var plainText = Encoding.UTF8.GetBytes(secretMessage);
        var cipherText = Encrypt(plainText, cryptKey, authKey, nonSecretPayload);
        return Convert.ToBase64String(cipherText);
    }

    /// <summary>
    /// Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.
    /// </summary>
    /// <param name="encryptedMessage">The encrypted message.</param>
    /// <param name="cryptKey">The crypt key.</param>
    /// <param name="authKey">The auth key.</param>
    /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
    /// <returns>
    /// Decrypted Message
    /// </returns>
    /// <exception cref="ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
    public string Decrypt(string encryptedMessage, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
    {
        if (string.IsNullOrWhiteSpace(encryptedMessage))
            throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

        var cipherText = Convert.FromBase64String(encryptedMessage);
        var plainText = Decrypt(cipherText, cryptKey, authKey, nonSecretPayloadLength);
        return plainText == null ? null : Encoding.UTF8.GetString(plainText);
    }

    /// <summary>
    /// Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message
    /// using Keys derived from a Password (PBKDF2).
    /// </summary>
    /// <param name="secretMessage">The secret message.</param>
    /// <param name="password">The password.</param>
    /// <param name="nonSecretPayload">The non secret payload.</param>
    /// <returns>
    /// Encrypted Message
    /// </returns>
    /// <exception cref="System.ArgumentException">password</exception>
    /// <remarks>
    /// Significantly less secure than using random binary keys.
    /// Adds additional non secret payload for key generation parameters.
    /// </remarks>
    public string EncryptWithPassword(string secretMessage, string password, byte[]? nonSecretPayload = null)
    {
        if (string.IsNullOrEmpty(secretMessage))
            throw new ArgumentException("Secret Message Required!", "secretMessage");

        var plainText = Encoding.UTF8.GetBytes(secretMessage);
        var cipherText = EncryptWithPassword(plainText, password, nonSecretPayload);
        return Convert.ToBase64String(cipherText);
    }

    /// <summary>
    /// Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message
    /// using keys derived from a password (PBKDF2). 
    /// </summary>
    /// <param name="encryptedMessage">The encrypted message.</param>
    /// <param name="password">The password.</param>
    /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
    /// <returns>
    /// Decrypted Message
    /// </returns>
    /// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
    /// <remarks>
    /// Significantly less secure than using random binary keys.
    /// </remarks>
    public string DecryptWithPassword(string encryptedMessage, string password, int nonSecretPayloadLength = 0)
    {
        if (string.IsNullOrWhiteSpace(encryptedMessage))
            throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

        var cipherText = Convert.FromBase64String(encryptedMessage);
        var plainText = DecryptWithPassword(cipherText, password, nonSecretPayloadLength);
        return plainText == null ? null : Encoding.UTF8.GetString(plainText);
    }

    /// <summary>
    /// Simple Encryption(AES) then Authentication (HMAC) for a UTF8 Message.
    /// </summary>
    /// <param name="secretMessage">The secret message.</param>
    /// <param name="cryptKey">The crypt key.</param>
    /// <param name="authKey">The auth key.</param>
    /// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
    /// <returns>
    /// Encrypted Message
    /// </returns>
    /// <remarks>
    /// Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +  HMac-Tag(32)) * 1.33 Base64
    /// </remarks>
    public byte[] Encrypt(byte[] secretMessage, byte[] cryptKey, byte[] authKey, byte[]? nonSecretPayload = null)
    {
        //User Error Checks
        if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
            throw new ArgumentException(string.Format("Key needs to be {0} bit!", KeyBitSize), nameof(cryptKey));

        if (authKey == null || authKey.Length != KeyBitSize / 8)
            throw new ArgumentException(string.Format("Key needs to be {0} bit!", KeyBitSize), nameof(authKey));

        if (secretMessage == null || secretMessage.Length < 1)
            throw new ArgumentException("Secret Message Required!", nameof(secretMessage));

        //non-secret payload optional
        nonSecretPayload = nonSecretPayload ?? Array.Empty<byte>();

        byte[] cipherText;
        byte[] iv;

        using (var aes = GetAes())
        {
            //Use random IV
            aes.GenerateIV();
            iv = aes.IV;

            using (var encrypter = aes.CreateEncryptor(cryptKey, iv))
            using (var cipherStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
                using (var binaryWriter = new BinaryWriter(cryptoStream))
                {
                    //Encrypt Data
                    binaryWriter.Write(secretMessage);
                }

                cipherText = cipherStream.ToArray();
            }

        }

        //Assemble encrypted message and add authentication
        using var hmac = new HMACSHA256(authKey);
        using var encryptedStream = new MemoryStream();
        using (var binaryWriter = new BinaryWriter(encryptedStream))
        {
            //Prepend non-secret payload if any
            binaryWriter.Write(nonSecretPayload);
            //Prepend IV
            binaryWriter.Write(iv);
            //Write Ciphertext
            binaryWriter.Write(cipherText);
            binaryWriter.Flush();

            //Authenticate all data
            var tag = hmac.ComputeHash(encryptedStream.ToArray());
            //Postpend tag
            binaryWriter.Write(tag);
        }
        return encryptedStream.ToArray();
    }

    /// <summary>
    /// Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.
    /// </summary>
    /// <param name="encryptedMessage">The encrypted message.</param>
    /// <param name="cryptKey">The crypt key.</param>
    /// <param name="authKey">The auth key.</param>
    /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
    /// <returns>Decrypted Message</returns>
    public byte[] Decrypt(byte[] encryptedMessage, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
    {

        //Basic Usage Error Checks
        if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
            throw new ArgumentException(string.Format("CryptKey needs to be {0} bit!", KeyBitSize), nameof(cryptKey));

        if (authKey == null || authKey.Length != KeyBitSize / 8)
            throw new ArgumentException(string.Format("AuthKey needs to be {0} bit!", KeyBitSize), nameof(authKey));

        if (encryptedMessage == null || encryptedMessage.Length == 0)
            throw new ArgumentException("Encrypted Message Required!", nameof(encryptedMessage));

        using var hmac = new HMACSHA256(authKey);
        var sentTag = new byte[hmac.HashSize / 8];
        //Calculate Tag
        var calcTag = hmac.ComputeHash(encryptedMessage, 0, encryptedMessage.Length - sentTag.Length);
        var ivLength = (BlockBitSize / 8);

        //if message length is to small just return null
        if (encryptedMessage.Length < sentTag.Length + nonSecretPayloadLength + ivLength)
            return null;

        //Grab Sent Tag
        Array.Copy(encryptedMessage, encryptedMessage.Length - sentTag.Length, sentTag, 0, sentTag.Length);

        //Compare Tag with constant time comparison
        var compare = 0;
        for (var i = 0; i < sentTag.Length; i++)
            compare |= sentTag[i] ^ calcTag[i];

        //if message doesn't authenticate return null
        if (compare != 0)
            return null;

        using var aes = GetAes();

        //Grab IV from message
        var iv = new byte[ivLength];
        Array.Copy(encryptedMessage, nonSecretPayloadLength, iv, 0, iv.Length);

        using (var decrypter = aes.CreateDecryptor(cryptKey, iv))
        using (var plainTextStream = new MemoryStream())
        {
            using (var decrypterStream = new CryptoStream(plainTextStream, decrypter, CryptoStreamMode.Write))
            using (var binaryWriter = new BinaryWriter(decrypterStream))
            {
                //Decrypt Cipher Text from Message
                binaryWriter.Write(encryptedMessage, nonSecretPayloadLength + iv.Length, encryptedMessage.Length - nonSecretPayloadLength - iv.Length - sentTag.Length);
            }
            //Return Plain Text
            return plainTextStream.ToArray();
        }
    }

    /// <summary>
    /// Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message
    /// using Keys derived from a Password (PBKDF2)
    /// </summary>
    /// <param name="secretMessage">The secret message.</param>
    /// <param name="password">The password.</param>
    /// <param name="nonSecretPayload">The non secret payload.</param>
    /// <returns>
    /// Encrypted Message
    /// </returns>
    /// <exception cref="System.ArgumentException">Must have a password of minimum length;password</exception>
    /// <remarks>
    /// Significantly less secure than using random binary keys.
    /// Adds additional non secret payload for key generation parameters.
    /// </remarks>
    public byte[] EncryptWithPassword(byte[] secretMessage, string password, byte[]? nonSecretPayload = null)
    {
        nonSecretPayload = nonSecretPayload ?? ""u8.ToArray();

        //User Error Checks
        if (password.Length < MinPasswordLength)
            throw new ArgumentException(string.Format("Must have a password of at least {0} characters!", MinPasswordLength), nameof(password));

        if (secretMessage == null || secretMessage.Length == 0)
            throw new ArgumentException("Secret Message Required!", nameof(secretMessage));

        var payload = new byte[((SaltBitSize / 8) * 2) + nonSecretPayload.Length];

        Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
        int payloadIndex = nonSecretPayload.Length;

        byte[] cryptKey;
        byte[] authKey;

        //Use Random Salt to prevent pre-generated weak password attacks.
        using (var generator = GetRfcDeriveBytes(password))
        {
            var salt = generator.Salt;

            //Generate Keys
            cryptKey = generator.GetBytes(KeyBitSize / 8);

            //Create Non Secret Payload
            Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
            payloadIndex += salt.Length;
        }

        //Deriving separate key, might be less efficient than using HKDF, 
        //but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
        using (var generator = GetRfcDeriveBytes(password))
        {
            var salt = generator.Salt;

            //Generate Keys
            authKey = generator.GetBytes(KeyBitSize / 8);

            //Create Rest of Non Secret Payload
            Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
        }

        return Encrypt(secretMessage, cryptKey, authKey, payload);
    }

    /// <summary>
    /// Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message
    /// using keys derived from a password (PBKDF2). 
    /// </summary>
    /// <param name="encryptedMessage">The encrypted message.</param>
    /// <param name="password">The password.</param>
    /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
    /// <returns>
    /// Decrypted Message
    /// </returns>
    /// <exception cref="System.ArgumentException">Must have a password of minimum length;password</exception>
    /// <remarks>
    /// Significantly less secure than using random binary keys.
    /// </remarks>
    public byte[] DecryptWithPassword(byte[] encryptedMessage, string password, int nonSecretPayloadLength = 0)
    {
        //User Error Checks
        if ( password.Length < MinPasswordLength)
            throw new ArgumentException(string.Format("Must have a password of at least {0} characters!", MinPasswordLength), nameof(password));

        if (encryptedMessage == null || encryptedMessage.Length == 0)
            throw new ArgumentException("Encrypted Message Required!", nameof(encryptedMessage));

        var cryptSalt = new byte[SaltBitSize / 8];
        var authSalt = new byte[SaltBitSize / 8];

        //Grab Salt from Non-Secret Payload
        Array.Copy(encryptedMessage, nonSecretPayloadLength, cryptSalt, 0, cryptSalt.Length);
        Array.Copy(encryptedMessage, nonSecretPayloadLength + cryptSalt.Length, authSalt, 0, authSalt.Length);

        byte[] cryptKey;
        byte[] authKey;

        //Generate crypt key
        using (var generator = GetRfcDeriveBytes(password, cryptSalt))
        {
            cryptKey = generator.GetBytes(KeyBitSize / 8);
        }
        //Generate auth key
        using (var generator = GetRfcDeriveBytes(password, authSalt))
        {
            authKey = generator.GetBytes(KeyBitSize / 8);
        }

        return Decrypt(encryptedMessage, cryptKey, authKey, cryptSalt.Length + authSalt.Length + nonSecretPayloadLength);
    }

    private Rfc2898DeriveBytes GetRfcDeriveBytes(string password)
    {
        return new Rfc2898DeriveBytes(password, SaltBitSize / 8, Iterations, HashAlgorithmName.SHA256);
    }

    private Rfc2898DeriveBytes GetRfcDeriveBytes(string password, byte[] salt)
    {
        return new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
    }

    private Aes GetAes()
    {
        var aes = Aes.Create();
        aes.KeySize = KeyBitSize;
        aes.BlockSize = BlockBitSize;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        return aes;
    }

}
