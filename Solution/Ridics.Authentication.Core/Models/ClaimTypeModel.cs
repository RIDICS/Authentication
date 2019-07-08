using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class ClaimTypeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ClaimTypeEnumModel Type { get; set; }

        public string Description { get; set; }

        public int SelectedType { get; set; }

        public IList<ClaimTypeEnumModel> AllTypeValues { get; set; }
    }
}