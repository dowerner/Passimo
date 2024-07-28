using Passimo.Domain.Model;

namespace Passimo.IO.IoEncoding.Cryptography;

public interface ICryptographicEncoder
{
    Task EncrypPasswordProfileAsync(PasswordProfile passwordProfile, System.IO.Stream targetStream, string password, CancellationToken ct = default);
    Task<System.IO.Stream> EncrypPasswordProfileAsync(PasswordProfile passwordProfile, string password, CancellationToken ct = default);
    Task<System.IO.Stream> EncryptStreamAsync(System.IO.Stream stream, string password, CancellationToken ct = default);
    Task EncryptStreamAsync(System.IO.Stream sourceStream, System.IO.Stream targetStream, string password, CancellationToken ct = default);
    Task<PasswordProfile> DecryptPasswordProfileAsync(System.IO.Stream stream, string password, CancellationToken ct = default);
}
