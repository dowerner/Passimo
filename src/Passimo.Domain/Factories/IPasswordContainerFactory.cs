using Passimo.Domain.Model;

namespace Passimo.Domain.Factories;

public interface IPasswordContainerFactory<T> where T : IPasswordContainer, new()
{
    T Create(string? name);
}

public interface IPasswordEntryFactory : IPasswordContainerFactory<PasswordEntry>
{
}

public interface IPasswordGroupFactory : IPasswordContainerFactory<PasswordGroup>
{
}
