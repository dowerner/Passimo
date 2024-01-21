using Passimo.Cryptography.Extensions;

namespace Passimo.Cryptography.Tests;

public class SecureStringAccessorTests
{
    [Theory]
    [InlineData("secret text")]
    [InlineData("S0me Sp3c!@l Ch@racters")]
    public void SecureStringsCanBeAccessed(string secret)
    {
        var secureString = CryptographyTestUtils.CreateSecureString(secret);
        using var accessor = secureString.Access();

        var recoveredSecret = string.Empty;
        accessor.GetValue(ref recoveredSecret);

        Assert.Equal(secret, recoveredSecret);
    }

    [Theory]
    [InlineData("secret text")]
    [InlineData("S0me Sp3c!@l Ch@racters")]
    public void SecureStringsNotAccessibleAfterDisposal(string secret)
    {
        var secureString = CryptographyTestUtils.CreateSecureString(secret);
        var accessor = secureString.Access();
        var recoveredSecret = string.Empty;
        accessor.GetValue(ref recoveredSecret);

        Assert.Equal(secret, recoveredSecret);

        accessor.Dispose();
        var accessAgain = string.Empty;

        Assert.Throws<ObjectDisposedException>(() => accessor.GetValue(ref accessAgain));
    }

    [Theory]
    [InlineData("secret text")]
    [InlineData("S0me Sp3c!@l Ch@racters")]
    public void SecureStringsAccessValueNotReadableAfterScramble(string secret)
    {
        var secureString = CryptographyTestUtils.CreateSecureString(secret);
        using var accessor = secureString.Access();

        var recoveredSecret = string.Empty;
        accessor.GetValue(ref recoveredSecret);

        Assert.Equal(secret, recoveredSecret);

        accessor.Scramble(ref recoveredSecret);

        Assert.NotEqual(secret, recoveredSecret);
    }
}
