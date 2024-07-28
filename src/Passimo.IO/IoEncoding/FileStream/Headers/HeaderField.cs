namespace Passimo.IO.IoEncoding.FileStream.Headers;

public interface IHeaderFiled
{
    uint Length { get; set; }
}

public  class HeaderField<T> : IHeaderFiled
{
    public Func<T> Getter { get; init; }
    public Action<T> Setter { get; init; }
    public uint Length { get; set; }
}
