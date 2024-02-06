using Passimo.Domain.Model;

namespace Passimo.TestUtils;

public static class PasswordProfileExtensions
{
    /// <summary>
    /// Returns the recursively calculated amount of entires in this profile.
    /// </summary>
    /// <param name="profile"></param>
    /// <returns></returns>
    public static int TotalStoredEntryCount(this PasswordProfile profile)
    {
        var count = 0;
        foreach (var entry in profile.Entries)
        {
            if (entry is PasswordGroup subGroup)
            {
                count += subGroup.TotalStoredEntryCount();
            }
            else if (entry is PasswordEntry)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Returns the recursively calculated amount of entires in this group.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public static int TotalStoredEntryCount(this PasswordGroup group)
    {
        var count = 0;
        foreach (var entry in group.ChildEntries)
        {
            if (entry is PasswordGroup subGroup)
            {
                count += subGroup.TotalStoredEntryCount();
            }
            else if (entry is PasswordEntry)
            {
                count++;
            }
        }
        return count;
    }
}
