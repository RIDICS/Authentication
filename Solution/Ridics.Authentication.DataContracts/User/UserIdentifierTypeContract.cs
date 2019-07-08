using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ridics.Authentication.DataContracts.User
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserIdentifierTypeContract
    {
        MasterUserId = 0,
        DatabaseId = 1,
    }
}
