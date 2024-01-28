namespace Passimo.IO.Core.Segments;

internal enum DataType : ushort
{
    Bool = 0,
    Byte = 1,
    Ushort = 2,
    Uint = 1,
    ByteArray = 51,
    String = 100,
    Guid = 101,
    DateTimeOffset = 102,
    List = 200,
    EntrySegment = 300,
    EntryGroupSegment = 301,
    ProfileSegment = 302,
    InfoEntryFieldSegment = 303,
    CryptographicEntryFieldSegment = 304
}
