using Passimo.Domain.Model;

namespace Passimo.Domain.Factories;

public class PasswordEntryFieldFactory : IPasswordEntryFieldFactory
{
    public PasswordEntryInfoField CreateDescription()
        => new PasswordEntryInfoField { Name = FactoryContstants.Description, NameLocalized = true, Type = PasswordEntryFiedType.LongText };

    public PasswordEntryInfoField CreateEmail()
        => new PasswordEntryInfoField { Name = FactoryContstants.Email, NameLocalized = true, Type = PasswordEntryFiedType.Text };

    public PasswordEntryCryptographicField CreatePassword()
        => new PasswordEntryCryptographicField { Name = FactoryContstants.Password, NameLocalized = true, Type = PasswordEntryFiedType.Password };

    public PasswordEntryInfoField CreateUrl()
        => new PasswordEntryInfoField { Name = FactoryContstants.Url, NameLocalized = true, Type = PasswordEntryFiedType.Text };

    public PasswordEntryInfoField CreateUsername()
        => new PasswordEntryInfoField { Name = FactoryContstants.Username, NameLocalized = true, Type = PasswordEntryFiedType.Text };
}
