namespace Ridics.Authentication.Core.Models
{
    public class ResourcePermissionTypeActionInfoModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ResourcePermissionTypeInfoModel ResourcePermissionType { get; set; }
    }
}