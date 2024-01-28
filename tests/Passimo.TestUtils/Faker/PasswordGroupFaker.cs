using Bogus;
using Passimo.Domain.Model;

namespace Passimo.TestUtils.Faker;

public class PasswordGroupFaker : Faker<PasswordGroup>
{
    public PasswordGroupFaker(string masterPassword, int entryCount, int subGroupCount, int maxDepth, int? seed = null)
    {
        if (seed is not null) UseSeed(seed.Value);

        var entryFaker = new PasswordEntryFaker(masterPassword, seed);
        var subGroupFaker = new PasswordGroupFaker(masterPassword, entryCount, subGroupCount, maxDepth - 1, seed);

        //TODO: Define rules for group generation
    }
}
