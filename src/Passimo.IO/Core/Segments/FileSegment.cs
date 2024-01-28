using System.Collections;
using System.Text;

namespace Passimo.IO.Core.Segments;

internal abstract class EncodingAction
{
}

internal abstract class DecodingAction
{
}

internal class EncodingEnterEncryptionContextAction : EncodingAction
{
    public EncryptionType EncryptionType { get; set; }
}

internal class EncodingExitEncryptionContextAction : EncodingAction { }

internal class EncodingDataAction : EncodingAction
{
    public DataType DataType { get; set; }
    public Func<byte[]> Getter { get; set; } = null!;
}


internal class EncodingListAction : EncodingDataAction
{
    public DataType ItemType { get; set; }
    public int ItemCount { get; set; }
}

internal interface IFileSegment
{
    void DefineSegment(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions);
}


internal abstract class FileSegment<T> : IFileSegment where T : class, new()
{
    public T EncodedObject { get; set; } = new();

    public List<EncodingAction> GetEncodingActions()
    {
        var encodingActions = new List<EncodingAction>();

        DefineSegment(encodingActions, null);

        return encodingActions;
    }
    

    public abstract void DefineSegment(List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions);

    protected void DefineSegmentField(
        DataType dataType, 
        Func<byte[]> getter, 
        Action<byte[]> setter, 
        List<EncodingAction>? encodingActions,
        List<DecodingAction>? decodingActions)
    {
        encodingActions?.Add(new EncodingDataAction
            {
                DataType = dataType,
                Getter = getter
            });
    }

    protected void DefineString(Func<string> getter, Action<string> setter, List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineSegmentField(
            DataType.String, 
            () => Encoding.UTF8.GetBytes(getter()), 
            value => setter(Encoding.UTF8.GetString(value)),
            encodingActions, decodingActions);
    }

    protected void DefineBool(Func<bool> getter, Action<bool> setter, List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineSegmentField(
            DataType.Bool,
            () => BitConverter.GetBytes(getter()),
            value => setter(BitConverter.ToBoolean(value)),
            encodingActions, decodingActions);
    }

    protected void DefineUshort(Func<ushort> getter, Action<ushort> setter, List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineSegmentField(
            DataType.Ushort,
            () => BitConverter.GetBytes(getter()),
            value => setter(BitConverter.ToUInt16(value)),
            encodingActions, decodingActions);
    }

    protected void DefineUint(Func<uint> getter, Action<uint> setter, List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineSegmentField(
            DataType.Uint,
            () => BitConverter.GetBytes(getter()),
            value => setter(BitConverter.ToUInt32(value)),
            encodingActions, decodingActions);
    }

    protected void DefineGuid(Func<Guid> getter, Action<Guid> setter, List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineSegmentField(
            DataType.Guid,
            () => getter().ToByteArray(),
            value => setter(new Guid(value)),
            encodingActions, decodingActions);
    }

    protected void DefineDateTimeOffset(Func<DateTimeOffset> getter, Action<DateTimeOffset> setter, List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineSegmentField(
            DataType.DateTimeOffset,
            () => BitConverter.GetBytes(getter().ToUnixTimeMilliseconds()),
            value => DateTimeOffset.FromUnixTimeMilliseconds(BitConverter.ToInt64(value)),
            encodingActions, decodingActions);
    }

    protected void DefineBytes(Func<byte[]> getter, Action<byte[]> setter, List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        DefineSegmentField(DataType.ByteArray, getter, setter, encodingActions, decodingActions);
    }

    protected void DefineList<TItem>(Func<List<TItem>> getter, Action<List<TItem>> setter, List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        if (encodingActions is not null)
        {
            // set list meta data
            encodingActions.Add(new EncodingListAction
            {
                DataType = DataType.List,
                ItemType = GetDataType<T>(),
                ItemCount = getter().Count
            });

            // encode list items
            var source = getter();

            switch (source)
            {
                case List<string> strList:
                    for (var i = 0; i < source.Count; ++i) DefineString(() => strList[i], v => strList[i] = v, encodingActions, null);
                    break;
                case List<uint> uintList:
                    for (var i = 0; i < source.Count; ++i) DefineUint(() => uintList[i], v => uintList[i] = v, encodingActions, null);
                    break;
                case List<ushort> ushortList:
                    for (var i = 0; i < source.Count; ++i) DefineUshort(() => ushortList[i], v => ushortList[i] = v, encodingActions, null);
                    break;
                case List<DateTimeOffset> dateTimeOffsetList:
                    for (var i = 0; i < source.Count; ++i) DefineDateTimeOffset(() => dateTimeOffsetList[i], v => dateTimeOffsetList[i] = v, encodingActions, null);
                    break;
                case List<byte[]> byteArrayList:
                    for (var i = 0; i < source.Count; ++i) DefineBytes(() => byteArrayList[i], v => byteArrayList[i] = v, encodingActions, null);
                    break;
                case List<IFileSegment> segmentList:
                    //todo: Does not work -> Segment encoding required
                    foreach (var segment in segmentList) segment.DefineSegment(encodingActions, null);
                    break;
            }
        }        
    }

    private static DataType GetDataType<TData>()
    {
        var type = typeof(TData);
        if (type == typeof(string)) return DataType.String;
        if (type == typeof(uint)) return DataType.Uint;
        if (type == typeof(ushort)) return DataType.Ushort;
        if (type == typeof(byte)) return DataType.Byte;
        if (type == typeof(DateTimeOffset)) return DataType.DateTimeOffset;
        if (type == typeof(Guid)) return DataType.Guid;
        if (type == typeof(byte[])) return DataType.ByteArray;
        if (type.IsSubclassOf(typeof(IList))) return DataType.List;

        //todo: Does not work -> Segment encoding required
        if (type.IsSubclassOf(typeof(IFileSegment))) return DataType.Segment;

        throw new ArgumentException($"The data type '{type.Name}' is not supported.");
    }

    protected void DefineWithEncryption(EncryptionType encryptionType, Action definitionAction, List<EncodingAction>? encodingActions, List<DecodingAction>? decodingActions)
    {
        if (encodingActions is not null)
        {
            encodingActions.Add(new EncodingEnterEncryptionContextAction
            {
                EncryptionType = encryptionType
            });
            definitionAction();
            encodingActions.Add(new EncodingExitEncryptionContextAction());
        }
    }
}
