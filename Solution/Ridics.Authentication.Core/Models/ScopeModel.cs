using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class ScopeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IList<ClaimTypeModel> Claims { get; set; }

        public bool Required { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }
    }
}