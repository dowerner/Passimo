using Passimo.Domain.Model;

namespace Passimo.Domain.Extensions;

public static class PasswordContainerExtensions
{
    public static T AsCustomField<T>(this T field, string customName) where T : PasswordEntryField
    {
        field.NameLocalized = false;
        field.Name = customName;
        field.Updated = DateTimeOffset.UtcNow;
        return field;
    }

    public static void UpdateValue(this PasswordEntryInfoField field, string value)
    {
        field.UpdateValue(value);
        field.Updated = DateTimeOffset.UtcNow;
    }
}
