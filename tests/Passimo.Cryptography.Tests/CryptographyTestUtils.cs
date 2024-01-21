using System.Security;

namespace Passimo.Cryptography.Tests;

internal static class CryptographyTestUtils
{
    public static SecureString CreateSecureString(string stringToSecure)
    {
        var secureString = new SecureString();
        foreach (var c in stringToSecure)
        {
            secureString.AppendChar(c);
        }
        return secureString;
    }
}
