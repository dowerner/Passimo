using Passimo.Domain.Model;
using Passimo.IO.Core.FileEncoding;
using Passimo.TestUtils;
using System.Text.Json;

namespace Passimo.IO.Tests;

public class SaveTests
{
    //[Theory]
    //[MemberData(nameof(FakeProfileData))]
    //public void SavingWorksAsExpected(PasswordProfile profile)
    //{

    //}

    [Fact]
    public void Test()
    {
        //var profile = Fakes.FakeSmallProfile;
        var profile = Fakes.FakeProfile;

        var entryCount = profile.TotalStoredEntryCount();

        ToJson(profile);
        var loadedProfile = FromJson();
        
        var loadedEntryCount = loadedProfile.TotalStoredEntryCount();

        var encoder = new PasswordProfileEncoder(profile);
        encoder.Encode();

        var test = profile;
    }

    private static void ToJson(PasswordProfile profile)
    {
        //var ms = new MemoryStream();
        using var ms = File.Create("test.json");
        JsonSerializer.Serialize(ms, profile);
    }

    private static PasswordProfile FromJson()
    {
        using var ms = File.OpenRead("test.json");
        return JsonSerializer.Deserialize<PasswordProfile>(ms)!;
    }

    //public static IEnumerable<object[]> FakeProfileData => [[Fakes.FakeProfile]];
}
