using Bogus;
using Passimo.Domain.Model;

namespace Passimo.TestUtils.Faker;

public class PasswordGroupFaker : Faker<PasswordGroup>, IEntryFaker
{
    private readonly DateTimeOffset _minDate = new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private readonly DateTimeOffset _maxDate = new DateTimeOffset(2024, 12, 31, 0, 0, 0, TimeSpan.Zero);

    public PasswordGroupFaker(Func<Bogus.Faker, byte[]> getFakePw, int minEntries, int maxEntries, int maxDepth, int? seed = null, double maxGroupFraction=0.2)
    {
        if (seed is not null) UseSeed(seed.Value);

        RuleFor(g => g.Created, f => f.Date.BetweenOffset(_minDate, _maxDate));
        RuleFor(g => g.Updated, (f, g) => f.Date.BetweenOffset(g.Created, _maxDate));
        RuleFor(g => g.Name, f => f.Name.JobArea());
        RuleFor(g => g.Fields, (f, g) => new List<PasswordEntryInfoField>
        {
            new() { Name = "PasswordContainer_Description", NameLocalized = true, Type = PasswordEntryFiedType.LongText, Value = $"{g.Name} group, contains more entries." },
            new() { Name = "PasswordContainer_Username", NameLocalized = true, Type = PasswordEntryFiedType.Text, Value = "miriam.muster" },
            new() { Name = "PasswordContainer_Email", NameLocalized = true, Type = PasswordEntryFiedType.Text, Value = "miriam.muster@mail.com" },
            new() { Name = "PasswordContainer_Url", NameLocalized = true, Type = PasswordEntryFiedType.Text }
        });
        RuleFor(g => g.ChildEntries, f =>
        {
            var entryCount = f.Random.Int(minEntries, maxEntries);
            var groupFraction = f.Random.Double(0, maxGroupFraction);
            var groupEntryCount = (int)(maxDepth > 0 ? groupFraction * entryCount : 0);
            var normalEntryCount = entryCount - groupEntryCount;

            int? childSeed = seed is not null ? f.Random.Int(seed.Value, 2 * seed.Value) : null;

            var entryFaker = new PasswordEntryFaker(getFakePw, childSeed);
            var entries = entryFaker.GenerateEntries(normalEntryCount);

            if (groupEntryCount > 0)
            {
                var subGroupFaker = new PasswordGroupFaker(getFakePw, minEntries, maxEntries, maxDepth - 1, childSeed);
                var groupEntries = subGroupFaker.GenerateEntries(groupEntryCount);
                entries.AddRange(groupEntries);
            }

            return entries;
        });
    }

    public List<IPasswordContainer> GenerateEntries(int count)
        => Generate(count).Cast<IPasswordContainer>().ToList();
}
