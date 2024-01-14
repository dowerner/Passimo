using Passimo.Domain.Model;

namespace Passimo.Domain.Extensions;

public static class PasswordContainerExtensions
{
    public static T AsCustomField<T>(this T field, string customName) where T : PasswordEntryField
    {
        field.NameLocalized = false;
        field.Name = customName;
        return field;
    }
}
