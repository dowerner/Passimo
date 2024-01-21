using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Passimo.Cryptography.Encryption;

namespace Passimo.Cryptography.Tests;

public abstract class EncryptionTestsBase
{
    protected abstract IEncryptionProcedure EncryptionProcedure { get; }

    [Theory]
    [InlineData("P@ssw0rd", "This message should be encrypted!")]
    public void SecretMessageGetEncrypedAndDecryptedAsExpected(string password, string message)
    {
        var securePassword = CryptographyTestUtils.CreateSecureString(password);
        var encrypted = EncryptionProcedure.EncryptWithPassword(message, securePassword);
        var decrypted = EncryptionProcedure.DecryptWithPassword(encrypted, securePassword);
        Assert.Equal(message, decrypted);
    }

    [Fact]
    public void BenchmarkTest()
    {
        var rnd = new Random();
        var size = 0;
        const int targetSize = 1024 * 1024 * 1024;
        const int chunkSize = 1024 * 1024 * 10;

        byte[] buffer = new byte[targetSize];

        var data = new int[targetSize / sizeof(int)];
        for (var i = 0; i < data.Length; ++i)
        {
            data[i] = i;
        }

        Buffer.BlockCopy(data, 0, buffer, 0, targetSize);

        var chunks = new List<byte[]>();
        var index = 0;

        while (index < buffer.Length)
        {
            var dataLength = Math.Min(buffer.Length - index - chunkSize, chunkSize);
            if (dataLength == 0) break;
            var chunk = new byte[dataLength];            
            Buffer.BlockCopy(buffer, index, chunk, 0, dataLength);
            chunks.Add(chunk);
            index += dataLength;
        }

        var securePassword = CryptographyTestUtils.CreateSecureString("P@ssw0rd!");
        var encryptedChunks = new byte[chunks.Count][];

        var test = EncryptionProcedure;

        Parallel.For(0, chunks.Count, i =>
        {
            var encrypted = EncryptionProcedure.EncryptWithPassword(chunks[i], securePassword);
            encryptedChunks[i] = encrypted;
        });

        var decryptedChunks = new byte[chunks.Count][];
        Parallel.For(0, chunks.Count, i =>
        {
            var decrypted = EncryptionProcedure.DecryptWithPassword(encryptedChunks[i], securePassword);
            decryptedChunks[i] = decrypted;
        });

        var decryptedBuffer = new byte[buffer.Length];
        index = 0;
        foreach (var chunk in decryptedChunks)
        {
            Buffer.BlockCopy(chunk, 0, decryptedBuffer, index, chunk.Length);
            index += chunk.Length;
        }

        var recoveredData = new int[data.Length];
        Buffer.BlockCopy(decryptedBuffer, 0, recoveredData, 0, targetSize);

        for (var i = 0; i < data.Length; ++i)
        {
            Assert.Equal(data[i], recoveredData[i]);
        }
    }
}

public class EncryptionAes128Tests : EncryptionTestsBase
{
    protected override IEncryptionProcedure EncryptionProcedure => EncryptionMethod.Aes128;
}

public class EncryptionAes256Tests : EncryptionTestsBase
{
    protected override IEncryptionProcedure EncryptionProcedure => EncryptionMethod.Aes256;
}