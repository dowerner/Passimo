namespace Passimo.IO.IoEncoding.FileStream.Headers;

public class FileHeader : HeaderBase
{
    public string FormatString { get; set; } = string.Empty;
    public ushort FileVersion { get; set; } = 1;
    
    protected override void DefineHeader()
    {
        DefineUtf8String(() => FormatString, s => FormatString = s);
        DefineUshort16(() => FileVersion, v => FileVersion = v);
    }
}
