using Passimo.Domain.Model;

namespace Passimo.IO.Core.Segments;

internal class EntrySegment : FileSegment<PasswordEntry>
{
    public override void DefineSegment(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineString(() => EncodedObject.Name, value => EncodedObject.Name = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Created, value => EncodedObject.Created = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Updated, value => EncodedObject.Updated = value, encodingActions, decodingActions);
        DefineList(() => EncodedObject.Fields, value => EncodedObject.Fields = value, encodingActions, decodingActions);
    }
}

internal class EntryGroupSegment : FileSegment<PasswordGroup>
{
    public override void DefineSegment(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineString(() => EncodedObject.Name, value => EncodedObject.Name = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Created, value => EncodedObject.Created = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Updated, value => EncodedObject.Updated = value, encodingActions, decodingActions);
        DefineList(() => EncodedObject.Fields, value => EncodedObject.Fields = value, encodingActions, decodingActions);
        DefineList(() => EncodedObject.ChildEntries, value => EncodedObject.ChildEntries = value, encodingActions, decodingActions);
    }
}
