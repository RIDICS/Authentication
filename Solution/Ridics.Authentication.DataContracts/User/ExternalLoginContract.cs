using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.DataContracts.User
{
    public class ExternalLoginContract
    {
        [Required]
        public int Id { get; set; }
        
        public int LoginProviderId { get; set; }
    }
}
