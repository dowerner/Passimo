using Passimo.Domain.Model;
using Passimo.IO.Core.FileEncoding;
using Passimo.IO.IoEncoding.Cryptography;
using Passimo.IO.IoEncoding.FileStream;
using Passimo.IO.IoEncoding.Raw;
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

    [Fact]
    public async Task TestNewFormat()
    {
        var profile = Fakes.FakeProfile;
        var cryptographicEncoder = GetCryptographicEncoder();

        var entryCount = profile.TotalStoredEntryCount();

        const string password = "P@ssW0rd";
        const string targetFile = "pwfile.pass";

        using (var ws = File.Create(targetFile))
        {
            await cryptographicEncoder.EncrypPasswordProfileAsync(profile, ws, password);
        }

        using (var rs = File.OpenRead(targetFile))
        {
            var restoredProfile = await cryptographicEncoder.DecryptPasswordProfileAsync(rs, password);
            var loadedEntryCount = restoredProfile.TotalStoredEntryCount();
        }
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

    private static ICryptographicEncoder GetCryptographicEncoder()
    {
        var rawEncoderDecoder = new RawJsonEncoderDecoder<PasswordProfile>();
        var fileEncoderDecoder = new FileStreamEncoderDecoder();
        return new CryptographicEncoder(rawEncoderDecoder, fileEncoderDecoder);
    }

    //public static IEnumerable<object[]> FakeProfileData => [[Fakes.FakeProfile]];
}
