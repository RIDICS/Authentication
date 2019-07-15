using System.Collections.Generic;

namespace Ridics.Authentication.DataContracts
{
    public class EnsurePermissionsContract : ContractBase
    {
        public IList<PermissionContractBase> Permissions { get; set; }

        public string NewAssignToRoleName { get; set; }
    }
}