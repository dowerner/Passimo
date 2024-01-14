using Passimo.IO.ProfileConnections.Local;
using Passimo.IO.Exceptions;

namespace Passimo.IO.ProfileConnections;

public class ProfileConnectionFactory : IProfileConnectionFactory
{
    public IPasswordProfileConnection Create(IPasswordProfileConnectionConfig config)
    {
        return config switch
        {
            LocalPasswordProfileConnectionConfig localConfig => new LocalPasswordProfileConnection(localConfig.LocalPath),
            _ => throw new ConnectionNotSupportedException($"There is no valid connection type for config '{config.GetType().Name}'")
        };
    }
}
