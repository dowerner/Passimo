
namespace Passimo.IO.ProfileConnections.Local;

public class LocalPasswordProfileConnectionConfig : IPasswordProfileConnectionConfig
{
    public string LocalPath { get; set; } = string.Empty;
    public Guid ProfileGuid { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public bool IsNameLocalized { get; set; }
}
