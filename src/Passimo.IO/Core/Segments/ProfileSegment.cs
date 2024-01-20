using Passimo.Domain.Model;

namespace Passimo.IO.Core.Segments;

internal class ProfileSegment : FileSegment<PasswordProfile>
{
    // Header info for the decoder to recognize the file
    public const string FixedTypeString = "PASSIMO";
    public const uint CurrentFileModelVersion = 1;

    public string TypeString { get; set; } = string.Empty;
    public uint FileModelVersion { get; set; }

    protected override void DefineSegment()
    {
        // unencrypted profile info
        DefineString(() => FixedTypeString, value => TypeString = value);
        DefineUint(() => CurrentFileModelVersion, value => FileModelVersion = value);
        DefineGuid(() => EncodedObject.ProfileGuid, value => EncodedObject.ProfileGuid = value);
        DefineString(() => EncodedObject.ProfileName, value => EncodedObject.ProfileName = value);

        // encrypted profile info fields
        DefineWithEncryption(EncryptionType.Aes256, () =>
        {
            DefineList(() => EncodedObject.Fields, value => EncodedObject.Fields = value);
        });

        // encrypted child entires
        DefineWithEncryption(EncryptionType.Aes256, () =>
        {
            DefineList(() => EncodedObject.Entries, value =>  EncodedObject.Entries = value);
        });
    }
}
