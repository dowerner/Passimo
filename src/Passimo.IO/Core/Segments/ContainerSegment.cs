using Passimo.Domain.Model;

namespace Passimo.IO.Core.Segments;

internal class EntrySegment : SegmentBase<PasswordEntry>
{
    public override DataType SegmentType => DataType.EntrySegment;

    protected override void DefineSegmentStructure(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineString(() => EncodedObject.Name, value => EncodedObject.Name = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Created, value => EncodedObject.Created = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Updated, value => EncodedObject.Updated = value, encodingActions, decodingActions);
        DefineList(() => EncodedObject.Fields, value => EncodedObject.Fields = value, encodingActions, decodingActions);
    }
}

internal class EntryGroupSegment : SegmentBase<PasswordGroup>
{
    public override DataType SegmentType => DataType.EntryGroupSegment;

    protected override void DefineSegmentStructure(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineString(() => EncodedObject.Name, value => EncodedObject.Name = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Created, value => EncodedObject.Created = value, encodingActions, decodingActions);
        DefineDateTimeOffset(() => EncodedObject.Updated, value => EncodedObject.Updated = value, encodingActions, decodingActions);
        DefineList(() => EncodedObject.Fields, value => EncodedObject.Fields = value, encodingActions, decodingActions);
        DefineList(() => EncodedObject.ChildEntries, value => EncodedObject.ChildEntries = value, encodingActions, decodingActions);
    }
}
