using Passimo.Domain.Model;

namespace Passimo.IO.Core.Segments;

internal abstract class EntryFieldSegment<T> : FileSegment<T> where T : PasswordEntryField, new()
{
    protected override void DefineSegment()
    {
        DefineString(() => EncodedObject.Name, value => EncodedObject.Name = value);
        DefineBool(() => EncodedObject.NameLocalized, value => EncodedObject.NameLocalized = value);
        DefineUshort(() => (ushort)EncodedObject.Type, value => EncodedObject.Type = (PasswordEntryFiedType)value);
        DefineDateTimeOffset(() => EncodedObject.Created, value => EncodedObject.Created = value);
        DefineDateTimeOffset(() => EncodedObject.Updated, value => EncodedObject.Updated = value);
    }
}

internal abstract class InfoEntryFieldSegment : EntryFieldSegment<PasswordEntryInfoField>
{
    protected override void DefineSegment()
    {
        base.DefineSegment();
        DefineString(() => EncodedObject.Value, value =>  EncodedObject.Value = value);
        DefineBool(() => EncodedObject.ValueLocalized, value => EncodedObject.ValueLocalized = value);
    }
}

internal abstract class PasswordEntryFieldSegment : EntryFieldSegment<PasswordEntryCryptographicField>
{
    protected override void DefineSegment()
    {
        base.DefineSegment();
        //EncodedObject.Password.
    }
}
