using Bogus;
using Passimo.Domain.Model;

namespace Passimo.TestUtils.Faker;

public class PasswordProfileFaker : Faker<PasswordProfile>
{
    public PasswordProfileFaker(string masterPassword, int rootEntryCount, int rootGroupCount, int? seed = null)
    { 
    }
}
