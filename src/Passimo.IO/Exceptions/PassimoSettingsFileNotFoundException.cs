namespace Passimo.IO.Exceptions;

public class PassimoSettingsFileNotFoundException : Exception
{
    public string Filename { get; set; } = string.Empty;

    public PassimoSettingsFileNotFoundException(string filename, string message)
        : base(message)
    {
        Filename = filename;
    }
}
