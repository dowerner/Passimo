namespace Passimo.IO.Core.Segments;

internal class ProfileSegment : FileSegment
{
    public const string FixedTypeString = "PASSIMO";
    public const uint CurrentFileModelVersion = 1;
    public string TypeString { get; private set; } = string.Empty;
    public uint FileModelVersion { get; private set; }
    public Guid ProfileGuid { get; set; }
    public string ProfileName { get; set; } = string.Empty;

    protected override void DefineSegment()
    {
        // unencrypted profile info
        DefineString(() => FixedTypeString, value => TypeString = value);
        DefineUint(() => CurrentFileModelVersion, value => FileModelVersion = value);
        DefineGuid(() => ProfileGuid, value => ProfileGuid = value);
        DefineString(() => ProfileName, value => ProfileName = value);

        // encrypted profile info fields
        DefineWithEncryption(EncryptionType.Aes256, () =>
        {

        });

        // encrypted child entires
        DefineWithEncryption(EncryptionType.Aes256, () =>
        {

        });
    }
}
