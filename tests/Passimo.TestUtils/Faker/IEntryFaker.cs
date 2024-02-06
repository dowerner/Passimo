using Passimo.Domain.Model;

namespace Passimo.TestUtils.Faker;

internal interface IEntryFaker
{
    List<IPasswordContainer> GenerateEntries(int count);
}
