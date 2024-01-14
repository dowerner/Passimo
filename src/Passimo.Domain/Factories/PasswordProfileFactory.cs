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
            ProfileGuid = Guid.NewGuid(),
            ProfileName = name,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            Fields = [ _fieldFactory.CreateDescription() ]
        };
    }

    public PasswordProfile CreateDefault()
    {
        var defaultProfile = Create(FactoryContstants.DefaultProfileName);
        defaultProfile.NameLocalized = true;
        return defaultProfile;
    }
}
