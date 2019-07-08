using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Models
{
    public class DataSourceModel
    {
        public int Id { get; protected set; }

        public DataSourceEnumModel DataSource { get; set; }

        public ExternalLoginProviderModel ExternalLoginProvider { get; set; }
    }
}
