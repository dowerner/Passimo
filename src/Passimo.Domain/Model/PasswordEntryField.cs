namespace Passimo.Domain.Model;

public enum PasswordEntryFiedType
{
    Text = 1,
    Link = 2,
    Password = 3,
    LongText = 4
}

public abstract class PasswordEntryField
{
    public PasswordEntryField()
    {
        Created = DateTimeOffset.UtcNow;
        Updated = DateTimeOffset.UtcNow;
    }

    public string Name { get; set; } = string.Empty;
    public bool NameLocalized { get; set; }
    public PasswordEntryFiedType Type { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
}

public class PasswordEntryInfoField : PasswordEntryField
{
    public string Value { get; set; } = string.Empty;
    public bool ValueLocalized { get; set; }
}

public class PasswordEntryCryptographicField : PasswordEntryField
{

}
