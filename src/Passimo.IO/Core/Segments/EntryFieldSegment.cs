using Passimo.Domain.Model;

namespace Passimo.IO.Core.Segments;

internal abstract class EntryFieldSegment<T> : SegmentBase<T> where T : PasswordEntryField, new()
{
    protected override void DefineSegmentStructure(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineString(() => EncodedObject.Name, value => EncodedObject.Name = value, encodingActions, decodingActions);
        DefineBool(() => EncodedObject.NameLocalized, value => EncodedObject.NameLocalized = value, encodingActions, decodingActions);
        DefineUshort(() => (ushort)EncodedObject.Type, value => EncodedObject.Type = (PasswordEntryFiedType)value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Created, value => EncodedObject.Created = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Updated, value => EncodedObject.Updated = value, encodingActions, decodingActions);
    }
}

internal class InfoEntryFieldSegment : EntryFieldSegment<PasswordEntryInfoField>
{
    public override DataType SegmentType => DataType.InfoEntryFieldSegment;

    protected override void DefineSegmentStructure(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        base.DefineSegmentStructure(encodingActions, decodingActions);
        DefineString(() => EncodedObject.Value, value =>  EncodedObject.Value = value, encodingActions, decodingActions);
        DefineBool(() => EncodedObject.ValueLocalized, value => EncodedObject.ValueLocalized = value, encodingActions, decodingActions);
    }
}

internal class CryptographicEntryFieldSegment : EntryFieldSegment<PasswordEntryCryptographicField>
{
    public override DataType SegmentType => DataType.CryptographicEntryFieldSegment;

    protected override void DefineSegmentStructure(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        base.DefineSegmentStructure(encodingActions, decodingActions);
        DefineBytes(() => EncodedObject.EncryptedPassword, value =>  EncodedObject.EncryptedPassword = value, encodingActions, decodingActions);
    }
}
