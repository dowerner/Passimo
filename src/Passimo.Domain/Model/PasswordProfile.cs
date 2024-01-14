namespace Passimo.Domain.Model;

public class PasswordProfile
{
    public string Name { get; set; } = string.Empty;
    public List<PasswordEntryInfoField> Fields { get; set; } = new();
    public List<IPasswordContainer> Entries { get; set; } = new();
}
