using System.Text;

namespace Passimo.IO.IoEncoding.FileStream.Headers;

public abstract class HeaderBase
{
    private readonly List<IHeaderFiled> _fields = [];

    protected abstract void DefineHeader();

    public static T Create<T>() where T : HeaderBase, new()
    {
        var header = new T();
        header.DefineHeader();
        return header;
    }

    public async Task WriteToStream(System.IO.Stream stream, CancellationToken ct = default)
    {
        // update lengths of dynamic fields
        foreach (var dynamicField in _fields.Where(f => f is HeaderField<string>).Cast<HeaderField<string>>())
        {
            // length is length of text in UTF8 bytes and the text legnth info (additional 4 bytes)
            dynamicField.Length = (uint) dynamicField.Getter().Length + sizeof(uint);
        }

        // write header length including the value itself
        var headerLength = (uint)(_fields.Sum(f => f.Length) + sizeof(uint));
        await stream.WriteAsync(BitConverter.GetBytes(headerLength), ct);

        // write header fields
        foreach (var field in _fields)
        {
            byte[] buffer;
            switch (field)
            {
                case HeaderField<ushort> ushortField:
                    buffer = BitConverter.GetBytes(ushortField.Getter());
                    break;
                case HeaderField<uint> uintField:
                    buffer = BitConverter.GetBytes(uintField.Getter());
                    break;
                case HeaderField<string> stringField:
                    buffer = Encoding.UTF8.GetBytes(stringField.Getter());
                    await stream.WriteAsync(BitConverter.GetBytes((uint)buffer.Length), ct);
                    break;
                default:
                    throw new ArgumentException("Field type cannot be written to stream");
            }

            await stream.WriteAsync(buffer, ct);
        }
    }

    public async Task ReadFromStream(System.IO.Stream stream, CancellationToken ct = default)
    {
        var headerStart = stream.Position;

        // read header length
        var headerLength = await ReadUint32(stream, ct);

        // read field values from stream
        foreach (var field in _fields)
        {
            switch (field)
            {
                case HeaderField<ushort> ushortField:
                    var ushortValue = await ReadUshort16(stream, ct);
                    ushortField.Setter(ushortValue);
                    break;
                case HeaderField<uint> uintField:
                    var uintValue = await ReadUint32(stream, ct);
                    uintField.Setter(uintValue);
                    break;
                case HeaderField<string> stringField:
                    var textLength = await ReadUint32(stream, ct);
                    var text = await ReadUtf8String(stream, textLength, ct);
                    stringField.Setter(text);
                    break;
                default:
                    throw new ArgumentException("Field type cannot be read from stream");
            }
        }

        // ensure that stream is advanced to header end
        var bytesRead = stream.Position - headerStart;
        var bytesToSkip = headerLength - bytesRead;
        if (bytesToSkip > 0) stream.Seek(headerStart + headerLength, SeekOrigin.Begin);
    }

    protected void DefineUint32(Func<uint> getter, Action<uint> setter)
        => DefineField(getter, setter, sizeof(uint));

    protected void DefineUshort16(Func<ushort> getter, Action<ushort> setter)
        => DefineField(getter, setter, sizeof(ushort));

    protected void DefineUtf8String(Func<string> getter, Action<string> setter)
    {
        var text = getter() ?? string.Empty;
        var valueLength = (uint) text.Length;
        DefineField(getter, setter, valueLength);
    }

    protected void DefineField<T>(Func<T> getter, Action<T> setter, uint length)
    {
        _fields.Add(new HeaderField<T> { Getter = getter, Setter = setter, Length = length });
    }

    private static async Task<uint> ReadUint32(System.IO.Stream stream, CancellationToken ct)
    {
        return BitConverter.ToUInt32(await ReadFromStreamInternal(stream, sizeof(uint), ct));
    }

    private static async Task<ushort> ReadUshort16(System.IO.Stream stream, CancellationToken ct)
    {
        return BitConverter.ToUInt16(await ReadFromStreamInternal(stream, sizeof(ushort), ct));
    }

    private static async Task<string> ReadUtf8String(System.IO.Stream stream, uint length, CancellationToken ct)
    {
        return Encoding.UTF8.GetString(await ReadFromStreamInternal(stream, (int)length, ct));
    }

    private static async Task<byte[]> ReadFromStreamInternal(System.IO.Stream stream, int length, CancellationToken ct)
    {
        var buffer = new byte[length];
        await stream.ReadAsync(buffer, 0, length, ct);
        return buffer;
    }
}
