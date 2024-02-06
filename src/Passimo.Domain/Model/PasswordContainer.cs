using System.Text.Json.Serialization;

namespace Passimo.Domain.Model;

[JsonDerivedType(typeof(PasswordEntry), typeDiscriminator: "E")]
[JsonDerivedType(typeof(PasswordGroup), typeDiscriminator: "G")]
public interface IPasswordContainer
{
    string Name { get; set; }
    DateTimeOffset Created { get; set; }
    DateTimeOffset Updated { get; set; }
}


public abstract class PasswordContainer<T> : IPasswordContainer where T : PasswordEntryField
{
    public string Name { get; set; } = string.Empty;
    public List<T> Fields { get; set; } = new();
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
}
