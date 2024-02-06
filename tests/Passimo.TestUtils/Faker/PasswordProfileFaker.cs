using Bogus;
using Passimo.Domain.Model;
using Passimo.Cryptography.Encryption;
using System.Text;

namespace Passimo.TestUtils.Faker;

public class PasswordProfileFaker : Faker<PasswordProfile>
{
    const int PasswordCount = 100;

    private readonly DateTimeOffset _minDate = new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private readonly DateTimeOffset _maxDate = new DateTimeOffset(2024, 12, 31, 0, 0, 0, TimeSpan.Zero);
    private List<byte[]> _passwords = new();
    private readonly string _masterPassword;

    public PasswordProfileFaker(string masterPassword, int minEntries, int maxEntries, int maxDepth, int? seed = null, double maxGroupFraction = 0.2)
    {
        if (seed is not null) UseSeed(seed.Value);

        _masterPassword = masterPassword;

        RuleFor(p => p.ProfileName, () => "MyProfile");
        RuleFor(p => p.ProfileGuid, f => f.Random.Guid());
        RuleFor(p => p.Created, f => f.Date.BetweenOffset(_minDate, _maxDate));
        RuleFor(p => p.Updated, (f, p) => f.Date.BetweenOffset(p.Created, _maxDate));
        RuleFor(p => p.Fields, (f, p) => new List<PasswordEntryInfoField>
        {
            new() { Name = "PasswordContainer_Description", NameLocalized = true, Type = PasswordEntryFiedType.LongText, Value = $"{p.ProfileName} is the default profile" },
            new() { Name = "PasswordContainer_Username", NameLocalized = true, Type = PasswordEntryFiedType.Text, Value = "miriam.muster" },
            new() { Name = "PasswordContainer_Email", NameLocalized = true, Type = PasswordEntryFiedType.Text, Value = "miriam.muster@mail.com" },
            new() { Name = "PasswordContainer_Url", NameLocalized = true, Type = PasswordEntryFiedType.Text }
        });

        // use group faker to generate entries
        var groupFaker = new PasswordGroupFaker(RandomPassword, minEntries, maxEntries, maxDepth, seed, maxGroupFraction);

        RuleFor(p => p.Entries, () => groupFaker.Generate().ChildEntries);
    }

    private byte[] RandomPassword(Bogus.Faker faker)
    {
        EnsurePasswordsAreCreated(faker);
        return faker.PickRandom(_passwords);
    }

    private void EnsurePasswordsAreCreated(Bogus.Faker faker)
    {
        if (_passwords.Count > 0) return;

        var encryption = EncryptionMethod.Aes256;

        for (var i = 0; i < PasswordCount; ++i)
        {
            var pw = faker.Internet.Password();

            var pwBytes = Encoding.UTF8.GetBytes(pw);
            var encryptedPw = encryption.EncryptWithPassword(pwBytes, _masterPassword);
            _passwords.Add(encryptedPw);
        }
    }
}
