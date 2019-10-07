using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.DataContracts
{
    public class PermissionContractBase : ContractBase
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class PermissionContract : PermissionContractBase
    {
        public List<RoleContractBase> Roles { get; set; }
    }
}
