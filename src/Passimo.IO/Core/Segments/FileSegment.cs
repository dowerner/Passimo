using Passimo.IO.Exceptions;
using System.Text;

namespace Passimo.IO.Core.Segments;

internal abstract class EncodingAction
{
}

internal class EncodingEnterEncryptionContextAction : EncodingAction
{
    public EncryptionType EncryptionType { get; set; }
}

internal class EncodingDataAction : EncodingAction
{
    public DataType DataType { get; set; }
    public Func<byte[]> Getter { get; set; } = null!;
}

internal abstract class FileSegment
{
    private List<EncodingAction>? _currentEncodingActions;

    public FileSegment? NextSegment { get; set; }

    public List<EncodingAction> GetEncodingActions()
    {
        if (_currentEncodingActions is not null)
            throw new PassimoEncodingException("Encoding already in progress.");

        _currentEncodingActions = new();

        DefineSegment();

        var result = _currentEncodingActions;
        _currentEncodingActions = null;

        return result;
    }
    

    protected abstract void DefineSegment();

    protected void DefineSegmentField(DataType dataType, Func<byte[]> getter, Action<byte[]> setter)
    {
        if (_currentEncodingActions is not null)
        {
            _currentEncodingActions.Add(new EncodingDataAction
            {
                DataType = dataType,
                Getter = getter
            });
        }
    }

    protected void DefineString(Func<string> getter, Action<string> setter)
    {
        DefineSegmentField(
            DataType.String, 
            () => Encoding.UTF8.GetBytes(getter()), 
            value => setter(Encoding.UTF8.GetString(value)));
    }

    protected void DefineUint(Func<uint> getter, Action<uint> setter)
    {
        DefineSegmentField(
            DataType.Uint,
            () => BitConverter.GetBytes(getter()),
            value => setter(BitConverter.ToUInt32(value)));
    }

    protected void DefineGuid(Func<Guid> getter, Action<Guid> setter)
    {
        DefineSegmentField(
            DataType.Guid,
            () => getter().ToByteArray(),
            value => setter(new Guid(value)));
    }

    protected void DefineWithEncryption(EncryptionType encryptionType, Action definitionAction)
    {
        if (_currentEncodingActions is not null)
        {
            _currentEncodingActions.Add(new EncodingEnterEncryptionContextAction
            {
                EncryptionType = encryptionType
            });
        }
    }
}
