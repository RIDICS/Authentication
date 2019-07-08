namespace Ridics.Authentication.DataEntities.Entities
{
    public class ClientSecretEntity : SecretEntity
    {
        public virtual ClientEntity Client { get; set; }
    }
}