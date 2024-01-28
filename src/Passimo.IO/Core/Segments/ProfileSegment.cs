using Passimo.Domain.Model;

namespace Passimo.IO.Core.Segments;

internal class ProfileSegment : FileSegment<PasswordProfile>
{
    // Header info for the decoder to recognize the file
    public const string FixedTypeString = "PASSIMO";
    public const uint CurrentFileModelVersion = 1;

    public string TypeString { get; set; } = string.Empty;
    public uint FileModelVersion { get; set; }

    public override void DefineSegment(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        // unencrypted profile info
        DefineString(() => FixedTypeString, value => TypeString = value, encodingActions, decodingActions);
        DefineUint(() => CurrentFileModelVersion, value => FileModelVersion = value, encodingActions, decodingActions);
        DefineGuid(() => EncodedObject.ProfileGuid, value => EncodedObject.ProfileGuid = value, encodingActions, decodingActions);
        DefineString(() => EncodedObject.ProfileName, value => EncodedObject.ProfileName = value, encodingActions, decodingActions);

        // encrypted profile info fields
        DefineWithEncryption(EncryptionType.Aes256, () =>
        {
            DefineList(() => EncodedObject.Fields, value => EncodedObject.Fields = value, encodingActions, decodingActions);
        }, encodingActions, decodingActions);

        // encrypted child entires
        DefineWithEncryption(EncryptionType.Aes256, () =>
        {
            DefineList(() => EncodedObject.Entries, value =>  EncodedObject.Entries = value, encodingActions, decodingActions);
        }, encodingActions, decodingActions);
    }
}
