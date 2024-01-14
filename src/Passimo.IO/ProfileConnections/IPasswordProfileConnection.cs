using Passimo.Domain.Model;

namespace Passimo.IO.ProfileConnections;

public interface IPasswordProfileConnection
{
    string ProfileName { get; set; }
    bool IsNameLocalized { get; set; }
    Guid ProfileGuid { get; set; }
    Task<PasswordProfile> GetProfile(CancellationToken token=default);
}
