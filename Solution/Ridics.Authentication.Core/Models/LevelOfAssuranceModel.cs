using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Models
{
    public class LevelOfAssuranceModel
    {
        public int Id { get; protected set; }

        public LevelOfAssuranceEnumModel Name { get; set; }

        public int Level { get; set; }
    }
}
