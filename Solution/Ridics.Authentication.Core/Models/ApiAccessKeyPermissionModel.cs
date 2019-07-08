namespace Ridics.Authentication.Core.Models
{
    public class ApiAccessKeyPermissionModel
    {
        public virtual int Id { get; protected set; }
        public virtual int Permission { get; set; }
    }
}