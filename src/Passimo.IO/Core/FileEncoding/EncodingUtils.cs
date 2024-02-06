using Passimo.IO.Core.Segments;
using System.Collections.Concurrent;

namespace Passimo.IO.Core.FileEncoding;

internal static class EncodingUtils
{
    private static readonly ConcurrentDictionary<DataType, byte[]> DataTypeTable = GenerateEnumTable<DataType>(d => BitConverter.GetBytes((ushort)d));

    public static byte[] ToBytes(this DataType dataType) => DataTypeTable[dataType];

    public static ConcurrentDictionary<TEnum, byte[]> GenerateEnumTable<TEnum>(Func<TEnum, byte[]> convert) where TEnum : struct, Enum
    {
        return new ConcurrentDictionary<TEnum, byte[]>(
            Enum.GetValues<TEnum>().Select(e => new KeyValuePair<TEnum, byte[]>(e, convert(e)))
        );
    }
}
