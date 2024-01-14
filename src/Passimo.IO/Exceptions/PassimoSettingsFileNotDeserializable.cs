namespace Passimo.IO.Exceptions;

public class PassimoSettingsFileNotDeserializable : Exception
{
    public string Filename { get; set; }
    public PassimoSettingsFileNotDeserializable(string filename, string message)
        : base(message) 
    {
        Filename = filename;
    }
}
