using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class ResourceModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<ClaimTypeModel> Claims { get; set; }

        public bool Required { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }
    }
}