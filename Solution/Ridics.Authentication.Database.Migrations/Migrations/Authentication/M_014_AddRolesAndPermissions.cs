using System.Collections.Generic;
using System.Linq;
using FluentMigrator;
using Ridics.Authentication.Database.Migrations.TagTypes;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.Authentication.Database.Migrations.Migrations.Authentication
{
    [DatabaseTags(DatabaseTagTypes.VokabularAuthDB)]
    [MigrationTypeTags(MigrationTypeTagTypes.Data, MigrationTypeTagTypes.All)]
    [Migration(014)]
    public class M_014_AddRolesAndPermissions : ForwardOnlyMigration
    {
        public M_014_AddRolesAndPermissions()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("Role")
                .Row(new { Name = "RegisteredUser" })
                .Row(new { Name = "VerifiedUser" })
                .Row(new { Name = "PortalAdmin", Description = "Role for Vokabular administrators" })
                .Row(new { Name = "Unregistered", Description = "Default user group" });

            Insert.IntoTable("Permission")
                .Row(new { Name = "upload-book", Description = "Permission to upload books" })
                //.Row(new { Name = "manage-permissions", Description = "Permission to manage permissions" })
                .Row(new { Name = "add-news", Description = "Permission to add news" })
                .Row(new { Name = "manage-feedbacks", Description = "Permission to manage feedbacks" })
                .Row(new { Name = "edit-lemmatization", Description = "Permission to edit lemmatization" })
                .Row(new { Name = "read-lemmatization", Description = "Permission to read lemmatization" })
                .Row(new { Name = "derivate-lemmatization", Description = "Permission to derivate lemmatization" })
                .Row(new { Name = "edition-print-text", Description = "Permission to " })
                .Row(new { Name = "edit-static-text", Description = "Permission to edit static text" })
                .Row(new { Name = "card-file:1", Description = "Permission to view card file NLA - exceprce" })
                .Row(new { Name = "card-file:2", Description = "Permission to view card file Gebauer – excerpce" })
                .Row(new { Name = "card-file:3", Description = "Permission to view card file Gebauer – prameny" })
                .Row(new { Name = "card-file:4", Description = "Permission to view card file Staročeská – excerpce" })
                .Row(new { Name = "card-file:5", Description = "Permission to view card file Zubatý – excerpce" })
                .Row(new { Name = "card-file:6", Description = "Permission to view card file Archiv lidového jazyka" })
                .Row(new { Name = "card-file:7", Description = "Permission to view card file Excerpce textu z 16. století" })
                .Row(new { Name = "card-file:8", Description = "Permission to view card file Latinsko-česká kartotéka" })
                .Row(new { Name = "card-file:9", Description = "Permission to view card file Rukopisy" })
                .Row(new { Name = "card-file:10", Description = "Permission to view card file Justitia" })
                .Row(new { Name = "card-file:11", Description = "Permission to view card file Tyl – excerpce" })
                .Row(new { Name = "card-file:12", Description = "Permission to view card file NLA - prameny" })
                .Row(new { Name = "card-file:13", Description = "Permission to view card file Svoboda" })
                .Row(new { Name = "card-file:14", Description = "Permission to view card file Excerpce pomístních jmen" })
                .Row(new { Name = "card-file:15", Description = "Permission to view card file Tereziánský katastr" })
                .Row(new { Name = "card-file:16", Description = "Permission to view card file Archivy, muzea" })
                .Row(new { Name = "card-file:17", Description = "Permission to view card file Stabilní katastr" })
                .Row(new { Name = "card-file:18", Description = "Permission to view card file Sajtl" })
                .Row(new { Name = "card-file:19", Description = "Permission to view card file Dodatky PSJC" })
                .Row(new { Name = "card-file:20", Description = "Permission to view card file Grepl - archiv" })
                .Row(new { Name = "autoimport:0", Description = "Permission to automatically obtain permission to view Editions" })
                .Row(new { Name = "autoimport:1", Description = "Permission to automatically obtain permission to view Dictionaries" })
                .Row(new { Name = "autoimport:2", Description = "Permission to automatically obtain permission to view Grammar" })
                .Row(new { Name = "autoimport:3", Description = "Permission to automatically obtain permission to view ProfessionalLiterature" })
                .Row(new { Name = "autoimport:4", Description = "Permission to automatically obtain permission to view Textbank" })
                .Row(new { Name = "autoimport:5", Description = "Permission to automatically obtain permission to view BibliographicalItems" })
                .Row(new { Name = "autoimport:6", Description = "Permission to automatically obtain permission to view CardFiles" })
                .Row(new { Name = "autoimport:7", Description = "Permission to automatically obtain permission to view AudioBook" });

            Execute.WithConnection((connection, transaction) =>
            {
                var portalAdminRoleId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "PortalAdmin")
                    .Run().Single();

                var adminRoleId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "Admin")
                    .Run().Single();


                var permissionNames = new List<string>
                {
                    "upload-book",
                    //"manage-permissions",
                    "add-news",
                    "manage-feedbacks",
                    "edit-lemmatization",
                    "read-lemmatization",
                    "derivate-lemmatization",
                    "edition-print-text",
                    "edit-static-text",
                    "card-file:1",
                    "card-file:2",
                    "card-file:3",
                    "card-file:4",
                    "card-file:5",
                    "card-file:6",
                    "card-file:7",
                    "card-file:8",
                    "card-file:9",
                    "card-file:10",
                    "card-file:11",
                    "card-file:12",
                    "card-file:13",
                    "card-file:14",
                    "card-file:15",
                    "card-file:16",
                    "card-file:17",
                    "card-file:18",
                    "card-file:19",
                    "card-file:20",
                    "autoimport:0",
                    "autoimport:1",
                    "autoimport:2",
                    "autoimport:3",
                    "autoimport:4",
                    "autoimport:5",
                    "autoimport:6",
                    "autoimport:7",
                };

                var insertQuery = Query.Conn(connection, transaction).Insert("Role_Permission");
                foreach (var permissionName in permissionNames)
                {
                    var permissionId = Query.Conn(connection, transaction).Select<int>("Id").From("Permission")
                        .Where("Name", permissionName)
                        .Run().Single();

                    insertQuery.Row(new { RoleId = portalAdminRoleId, PermissionId = permissionId });
                    insertQuery.Row(new { RoleId = adminRoleId, PermissionId = permissionId });
                }
                insertQuery.Run();
            });
        }
    }
}