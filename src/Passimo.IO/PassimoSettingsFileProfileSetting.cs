using Passimo.IO.ProfileConnections;

namespace Passimo.IO;

public class PassimoSettingsFileProfileSetting
{
    public IPasswordProfileConnectionConfig ConnectionConfig { get; set; } = null!;

    public List<IPasswordProfileConnectionConfig> BackupConnections { get; set; } = new();
}
