using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ExternalLoginProviderMapping : ClassMapping<ExternalLoginProviderEntity>, IMapping
    {
        public ExternalLoginProviderMapping()
        {
            Table("`ExternalLoginProvider`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.DisplayName, map =>
            {
                map.NotNullable(true);
            });

            Property(x => x.CreateNotExistUser, map =>
            {
                map.NotNullable(true);
            });

            Property(x => x.HideOnLoginScreen, map =>
            {
                map.NotNullable(true);
            });

            Property(x => x.MainColor);

            Property(x => x.DisableManagingByUser, map =>
            {
                map.NotNullable(true);
            });

            ManyToOne(x => x.Logo, map => { map.Column("`LogoId`"); });

            ManyToOne(x => x.DynamicModule, map => { map.Column("`DynamicModuleId`"); });
        }
    }
}
