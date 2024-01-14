namespace Passimo.IO.Services;

public interface IPassimoSettingsService
{
    Task<PassimoSettingsFile> LoadFromFile(string filename, CancellationToken token=default);

    bool Exist(string filename);

    Task SaveToFile(string filename, PassimoSettingsFile settings, CancellationToken token=default);

    Task<PassimoSettingsFile> CreateSettingsFile(string filename, CancellationToken token = default);

    Task<PassimoSettingsFile> LoadOrCreate(string filename, CancellationToken token = default);
}
