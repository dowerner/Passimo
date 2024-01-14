using Passimo.Domain.Factories;
using Passimo.IO.Exceptions;
using Passimo.IO.ProfileConnections.Local;
using System.Text.Json;

namespace Passimo.IO.Services;

public class PassimoSettingsService : IPassimoSettingsService

{
    private readonly IPasswordProfileFactory _profileFactory;

    public PassimoSettingsService(IPasswordProfileFactory profileFactory)
    {
        _profileFactory = profileFactory;
    }

    public async Task<PassimoSettingsFile> LoadFromFile(string filename, CancellationToken token = default)
    {
        if (!File.Exists(filename))
            throw new PassimoSettingsFileNotFoundException(filename, $"Unable to find settings file '{filename}'");

        await using var settingsStream = File.OpenRead(filename);

        var settings = await JsonSerializer.DeserializeAsync<PassimoSettingsFile>(settingsStream, cancellationToken: token);

        if (settings is null)
            throw new PassimoSettingsFileNotDeserializable(filename, $"Found file but got NULL when deserializing '{filename}'");

        return settings;
    }

    public bool Exist(string filename) => File.Exists(filename);

    public async Task SaveToFile(string filename, PassimoSettingsFile settings, CancellationToken token = default)
    {
        var backupPath = $"{filename}.{IoConstants.BackupExt}";

        // first, create a backup of the current settings file
        if (File.Exists(filename))
        {
            File.Copy(filename, backupPath, true);
            if (!File.Exists(backupPath))
                throw new UnableToSavePassimoSettingsFileException(filename, $"Unable to create pre-save backup for '{filename}'");
            File.Delete(filename);
        }

        // create a new settings file
        await using var settingsStream = File.Create(filename);

        try
        {
            // serialized the settings
            settings.Updated = DateTimeOffset.UtcNow;
            await JsonSerializer.SerializeAsync(settingsStream, settings, cancellationToken: token);
        }
        catch (Exception ex)
        {
            // if the serialization fails, try to restore the backup file
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, filename, true);
                File.Delete(backupPath);
            }
            var saveEx = new UnableToSavePassimoSettingsFileException(filename, $"Unable to save settings file '{filename}'. Backup restored.");
            saveEx.CausingException = ex;
            throw saveEx;
        }
    }

    public async Task<PassimoSettingsFile> CreateSettingsFile(string filename, CancellationToken token = default)
    {
        var defaultProfile = _profileFactory.CreateDefault();
        var localConnectionConfig = new LocalPasswordProfileConnectionConfig
        {
            LocalPath = filename,
            ProfileGuid = defaultProfile.ProfileGuid,
            ProfileName = defaultProfile.ProfileName,
            IsNameLocalized = defaultProfile.NameLocalized
        };
        var settings = new PassimoSettingsFile
        {
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            ActiveProfile = defaultProfile.ProfileGuid,
            Profiles = [new PassimoSettingsFileProfileSetting { ConnectionConfig = localConnectionConfig }]
        };

        await SaveToFile(filename, settings, token);

        return settings;
    }

    public async Task<PassimoSettingsFile> LoadOrCreate(string filename, CancellationToken token = default)
    {
        return Exist(filename) ? await LoadFromFile(filename, token) : await CreateSettingsFile(filename, token);
    }
}
