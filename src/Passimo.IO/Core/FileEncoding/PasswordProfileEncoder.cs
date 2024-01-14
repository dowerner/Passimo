using Passimo.Domain.Model;
using Passimo.IO.Core.Segments;

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
        var profileSegment = new ProfileSegment
        {
            ProfileGuid = _profile.ProfileGuid,
            ProfileName = _profile.ProfileName
        };

        var encodingActions = profileSegment.GetEncodingActions();
    }
}
