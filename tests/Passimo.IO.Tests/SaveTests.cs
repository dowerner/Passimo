using Passimo.Domain.Model;
using Passimo.TestUtils;

namespace Passimo.IO.Tests;

public class SaveTests
{
    [Theory]
    [MemberData(nameof(FakeProfileData))]
    public void SavingWorksAsExpected(PasswordProfile profile)
    {

    }

    [Fact]
    public void Test()
    {
        var profile = Fakes.FakeProfile;
        var entryCount = profile.TotalStoredEntryCount();
        var test = profile;
    }

    public static IEnumerable<object[]> FakeProfileData => [[Fakes.FakeProfile]];
}
