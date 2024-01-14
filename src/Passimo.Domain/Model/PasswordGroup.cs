namespace Passimo.Domain.Model;

public class PasswordGroup : PasswordContainer<PasswordEntryInfoField>
{
    public List<IPasswordContainer> ChildEntries { get; set; } = new();
}
