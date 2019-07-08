using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ridics.Authentication.Service.Models.API.UserActivation
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IdentifierType
    {
        MasterUserId,
    }
}