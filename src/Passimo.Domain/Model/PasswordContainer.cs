namespace Passimo.Domain.Model;
public interface IPasswordContainer
{
    string Name { get; set; }
}


public abstract class PasswordContainer<T> : IPasswordContainer where T : PasswordEntryField
{
    public string Name { get; set; } = string.Empty;
    public List<T> Fields { get; set; } = new();
}
