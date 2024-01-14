using Passimo.IO.ProfileConnections.Local;
using System.Text.Json.Serialization;

namespace Passimo.IO.ProfileConnections;

[JsonDerivedType(typeof(LocalPasswordProfileConnectionConfig), typeDiscriminator: "local")]
public interface IPasswordProfileConnectionConfig
{
    Guid ProfileGuid { get; set; }
    string ProfileName { get; set; }
    bool IsNameLocalized { get; set; }
}
