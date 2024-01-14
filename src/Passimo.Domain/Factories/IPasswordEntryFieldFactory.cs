using Passimo.Domain.Model;

namespace Passimo.Domain.Factories;

public interface IPasswordEntryFieldFactory
{
    PasswordEntryInfoField CreateDescription();
    PasswordEntryInfoField CreateUsername();
    PasswordEntryInfoField CreateEmail();
    PasswordEntryInfoField CreateUrl();
    PasswordEntryCryptographicField CreatePassword();
}
