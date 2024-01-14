using Passimo.Domain.Model;

namespace Passimo.Domain.Factories;

public class PasswordProfileFactory : IPasswordProfileFactory
{
    private readonly IPasswordEntryFieldFactory _fieldFactory;
    public PasswordProfileFactory(IPasswordEntryFieldFactory fieldFactory)
    {
        _fieldFactory = fieldFactory;
    }

    public PasswordProfile Create(string name)
    {
        return new PasswordProfile
        {
            Name = name,
            Fields = [
                _fieldFactory.CreateDescription()
            ]
        };
    }
}
