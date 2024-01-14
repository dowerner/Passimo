using Passimo.Domain.Model;

namespace Passimo.Domain.Factories;

public interface IPasswordProfileFactory
{
    PasswordProfile Create(string name);
}
