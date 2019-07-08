namespace Ridics.Authentication.DataEntities.Entities
{
    public class ApiSecretEntity : SecretEntity
    {
        public virtual ApiResourceEntity ApiResource { get; set; }
    }
}