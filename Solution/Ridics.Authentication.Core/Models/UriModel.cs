using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class UriModel
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Value { get; set; }

        public virtual IList<UriTypeModel> UriTypes { get; set; }
    }
}