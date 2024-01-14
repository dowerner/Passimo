namespace Passimo.IO.ProfileConnections;

public interface IProfileConnectionFactory
{
    IPasswordProfileConnection Create(IPasswordProfileConnectionConfig config);
}
