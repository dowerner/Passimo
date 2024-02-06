using Passimo.Domain.Model;
using Passimo.TestUtils.Faker;

namespace Passimo.IO.Tests;

internal static class Fakes
{
    public const string FakePassword = "P@ssW0rd";
    public const int FakeMinEntriesPerLevel = 100;
    public const int FakeMaxEntriesPerLevel = 1000;
    public const int FakeDepth = 1;
    public const int FakeSeed = 42;
    public const double FakeMaxGroupFraction = 0.1;

    public static readonly PasswordProfile FakeProfile = new PasswordProfileFaker(
        FakePassword, 
        FakeMinEntriesPerLevel, 
        FakeMaxEntriesPerLevel, 
        FakeDepth, 
        FakeSeed, 
        FakeMaxGroupFraction
    );
}
