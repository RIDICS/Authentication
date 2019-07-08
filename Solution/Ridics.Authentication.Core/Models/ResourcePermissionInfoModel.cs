namespace Ridics.Authentication.Core.Models
{
    public class ResourcePermissionInfoModel
    {
        public int Id { get; set; }

        public string ResourceId { get; set; }

        public ResourcePermissionTypeActionInfoModel ResourceTypeAction { get; set; }
    }
}