using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ClientMapping : ClassMapping<ClientEntity>, IMapping
    {
        public ClientMapping()
        {
            Table("`Client`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.Description);

            Set(x => x.Secrets, map =>
            {
                map.Table("`Secret`");
                map.Key(keyMapper => keyMapper.Column("`ClientId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());

            Set(x => x.AllowedGrantTypes, map =>
            {
                map.Table("`Client_GrantType`");
                map.Key(keyMapper => keyMapper.Column("`ClientId`"));
            }, rel => rel.ManyToMany(m => m.Column("`GrantTypeId`")));

            Set(x => x.UriList, map =>
            {
                map.Table("`Uri`");
                map.Key(keyMapper => keyMapper.Column("`ClientId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());

            Set(x => x.AllowedIdentityResources, map =>
            {
                map.Table("`Client_IdentityResource`");
                map.Key(keyMapper => keyMapper.Column("`ClientId`"));
            }, rel => rel.ManyToMany(m => m.Column("`IdentityResourceId`")));

            Set(x => x.AllowedScopes, map =>
            {
                map.Table("`Client_Scope`");
                map.Key(keyMapper => keyMapper.Column("`ClientId`"));
            }, rel => rel.ManyToMany(m => m.Column("`ScopeId`")));

            Property(x => x.DisplayUrl);

            Property(x => x.LogoUrl);

            Property(x => x.RequireConsent);

            Property(x => x.AllowAccessTokensViaBrowser);

            Set(x => x.PersistedGrants, map =>
            {
                map.Table("`PersistedGrant`");
                map.Key(keyMapper => keyMapper.Column("`ClientId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());
        }
    }
}