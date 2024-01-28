using Passimo.Domain.Model;

namespace Passimo.IO.Core.Segments;

internal abstract class EntryFieldSegment<T> : FileSegment<T> where T : PasswordEntryField, new()
{
    public override void DefineSegment(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineString(() => EncodedObject.Name, value => EncodedObject.Name = value, encodingActions, decodingActions);
        DefineBool(() => EncodedObject.NameLocalized, value => EncodedObject.NameLocalized = value, encodingActions, decodingActions);
        DefineUshort(() => (ushort)EncodedObject.Type, value => EncodedObject.Type = (PasswordEntryFiedType)value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Created, value => EncodedObject.Created = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Updated, value => EncodedObject.Updated = value, encodingActions, decodingActions);
    }
}

internal abstract class InfoEntryFieldSegment : EntryFieldSegment<PasswordEntryInfoField>
{
    public override void DefineSegment(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        base.DefineSegment(encodingActions, decodingActions);
        DefineString(() => EncodedObject.Value, value =>  EncodedObject.Value = value, encodingActions, decodingActions);
        DefineBool(() => EncodedObject.ValueLocalized, value => EncodedObject.ValueLocalized = value, encodingActions, decodingActions);
    }
}

internal abstract class PasswordEntryFieldSegment : EntryFieldSegment<PasswordEntryCryptographicField>
{
    public override void DefineSegment(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        base.DefineSegment(encodingActions, decodingActions);
        DefineBytes(() => EncodedObject.EncryptedPassword, value =>  EncodedObject.EncryptedPassword = value, encodingActions, decodingActions);
    }
}
