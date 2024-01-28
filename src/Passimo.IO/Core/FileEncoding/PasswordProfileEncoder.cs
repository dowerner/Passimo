using Passimo.Domain.Model;
using Passimo.IO.Core.Segments;
using System.Security;

namespace Passimo.IO.Core.FileEncoding;

public class PasswordProfileEncoder
{
    private readonly PasswordProfile _profile;

    public PasswordProfileEncoder(PasswordProfile profile)
    {
        _profile = profile;
    }

    public void Encode()
    {
        var profileSegment = new ProfileSegment { EncodedObject = _profile };

        var pw = new SecureString();
        const string pwClear = "secret";
        foreach (var c in pwClear)
        {
            pw.AppendChar(c);
        }

        var encodingActions = profileSegment.GetEncodingActions();
        Console.Write("test");
    }
}
