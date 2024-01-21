using System.Security;

namespace Passimo.Cryptography.Extensions;

public static class SecureStringExtensions
{
    public static SecureStringAccessor Access(this SecureString secureString)
        => new SecureStringAccessor(secureString);
}
