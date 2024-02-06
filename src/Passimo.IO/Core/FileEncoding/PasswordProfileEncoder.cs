using Passimo.Domain.Model;
using Passimo.IO.Core.Segments;
using System.Security;

namespace Passimo.IO.Core.FileEncoding;

public class PasswordProfileEncoder
{
    private readonly PasswordProfile _profile;
    private readonly Stack<StreamEncoder> _encoderStack = new();

    public PasswordProfileEncoder(PasswordProfile profile)
    {
        _profile = profile;
    }

    public void Encode()
    {
        var profileSegment = new ProfileSegment { EncodedObject = _profile };
        var encodingActions = profileSegment.GetEncodingActions();

        var encoder = new StreamEncoder();

        foreach (var encodingAction in encodingActions)
        {
            switch (encodingAction)
            {
                case EncodingDataAction encodingDataAction:
                    encoder.Stream.Write(encodingDataAction.DataType.ToBytes());
                    if (encodingDataAction is EncodingListAction encodingListAction)
                    {
                        encoder.Stream.Write(encodingListAction.ItemType.ToBytes());
                        encoder.Stream.Write(BitConverter.GetBytes(encodingListAction.ItemCount));
                        _encoderStack.Push(encoder);
                        encoder = new() { IsChildEncoder = true, ItemCount = encodingListAction.ItemCount };
                    }
                    else
                    {
                        var data = encodingDataAction.Getter();
                        encoder.Stream.Write(BitConverter.GetBytes(data.Length));
                        encoder.Stream.Write(data);
                    }
                    break;
                case EncodingSegmentStartAction encodingSegmentStartAction:
                    encoder.Stream.Write(encodingSegmentStartAction.SegmentType.ToBytes());
                    break;
                case EncodingSegmentEndAction encodingSegmentEndAction when encoder.IsChildEncoder:
                    ++encoder.CurrentItem;
                    if (encoder.CurrentItem >= encoder.ItemCount)
                    {
                        var childStream = encoder.Stream;
                        encoder = _encoderStack.Pop();
                        encoder.Stream.Write(childStream.GetBuffer());
                    }
                    break;
            }
        }

        var streamLength = encoder.Stream.Length;
    }
}
