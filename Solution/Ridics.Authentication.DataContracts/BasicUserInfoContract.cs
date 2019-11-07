using System.Collections.Generic;

namespace Ridics.Authentication.DataContracts
{
    public class BasicUserInfoContract : ContractBase
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public Dictionary<string, string> UserData { get; set; }
    }
}