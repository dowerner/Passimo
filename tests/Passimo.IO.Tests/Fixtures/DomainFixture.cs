using Passimo.Domain.Factories;

namespace Passimo.IO.Tests.Fixtures;

public class DomainFixture : IDisposable
{
    private readonly IServiceProvider _serviceProvider; 

    public IPasswordProfileFactory PasswordProfileFactory { get; init; }

    public DomainFixture()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IPasswordEntryFieldFactory, PasswordEntryFieldFactory>();
        services.AddSingleton<IPasswordProfileFactory, PasswordProfileFactory>();

        _serviceProvider = services.BuildServiceProvider();

        PasswordProfileFactory = _serviceProvider.GetRequiredService<IPasswordProfileFactory>();
    }

    public void Dispose()
    {
    }
}
