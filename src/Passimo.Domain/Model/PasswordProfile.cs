namespace Passimo.Domain.Model;

public class PasswordProfile
{
    public Guid ProfileGuid { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public bool NameLocalized { get; set; }
    public List<PasswordEntryInfoField> Fields { get; set; } = new();
    public List<IPasswordContainer> Entries { get; set; } = new();
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set;}
}
