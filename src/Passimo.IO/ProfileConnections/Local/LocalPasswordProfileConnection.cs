using Passimo.Domain.Model;

namespace Passimo.IO.ProfileConnections.Local;

public class LocalPasswordProfileConnection : IPasswordProfileConnection
{
    private readonly string _localPath;
    public string ProfileName { get; set; } = string.Empty;
    public bool IsNameLocalized { get; set; }
    public Guid ProfileGuid { get; set; }

    public LocalPasswordProfileConnection(string localPath)
    {
        _localPath = localPath;
    }

    public async Task<PasswordProfile> GetProfile(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
