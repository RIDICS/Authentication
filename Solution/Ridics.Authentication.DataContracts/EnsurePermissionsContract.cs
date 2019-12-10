using System.Collections.Generic;

namespace Ridics.Authentication.DataContracts
{
    public class EnsurePermissionsContract : ContractBase
    {
        public IList<PermissionContractBase> Permissions { get; set; }

        public IList<string> NewAssignToRoleNames { get; set; }
    }
}