using Passimo.IO.Core.Segments;

namespace Passimo.IO.Core.FileEncoding;

internal class StreamEncoder
{
    public MemoryStream Stream { get; } = new MemoryStream();
    public bool IsChildEncoder { get; set; }
    public int ItemCount { get; set; }
    public int CurrentItem { get; set; }
}
