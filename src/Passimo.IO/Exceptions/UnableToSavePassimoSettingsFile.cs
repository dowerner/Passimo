namespace Passimo.IO.Exceptions;

public class UnableToSavePassimoSettingsFileException : Exception
{
    public string Filename { get; set; }
    public Exception? CausingException { get; set; }

    public UnableToSavePassimoSettingsFileException(string filename, string message)
        : base(message)
    {
        Filename = filename;
    }
}
