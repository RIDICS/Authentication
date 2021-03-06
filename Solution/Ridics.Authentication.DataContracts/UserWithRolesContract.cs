﻿using System.Collections.Generic;

namespace Ridics.Authentication.DataContracts
{
    public class UserWithRolesContract : ContractBase
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<RoleContractBase> Roles { get; set; }
    }
}