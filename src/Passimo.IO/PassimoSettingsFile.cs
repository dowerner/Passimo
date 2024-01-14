namespace Passimo.IO;

public class PassimoSettingsFile
{
    public string? Local { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public Guid ActiveProfile { get; set; }
    public List<PassimoSettingsFileProfileSetting> Profiles { get; set; } = new();
}
