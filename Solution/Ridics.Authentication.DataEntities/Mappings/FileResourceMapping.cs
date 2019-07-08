using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class FileResourceMapping : ClassMapping<FileResourceEntity>, IMapping
    {
        public FileResourceMapping()
        {
            Table("`FileResource`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Guid, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.Type, map =>
            {
                map.NotNullable(true);
                map.Type<EnumStringType<FileResourceEnum>>();
            });

            Property(x => x.FileExtension, map =>
            {
                map.NotNullable(true);
            });
        }
    }
}
