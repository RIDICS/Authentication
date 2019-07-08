using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Ridics.Authentication.Modules.Shared.Model
{
    public class LevelOfAssuranceValue<T>
    {
        public LevelOfAssuranceValue(T value) : this(value, LevelOfAssuranceEnumModel.Low)
        {
        }

        public LevelOfAssuranceValue(T value, LevelOfAssuranceEnumModel levelOfAssurance)
        {
            Value = value;
            LevelOfAssurance = levelOfAssurance;
        }

        public static LevelOfAssuranceValue<string> FromClaim(IList<Claim> claims, string key, LevelOfAssuranceEnumModel levelOfAssurance)
        {
            return claims.Where(x => x.Type.Equals(key)).Select(x =>
                new LevelOfAssuranceValue<string>(
                    x.Value, levelOfAssurance
                )).FirstOrDefault();
        }

        public T Value { get; }

        public LevelOfAssuranceEnumModel LevelOfAssurance { get; }
    }

    public class LevelOfAssuranceValue
    {
        public static LevelOfAssuranceValue<string> FromClaim(
            IList<Claim> claims, string key, LevelOfAssuranceEnumModel levelOfAssurance=LevelOfAssuranceEnumModel.Low
            )
        {
            return claims.Where(x => x.Type.Equals(key)).Select(x =>
                new LevelOfAssuranceValue<string>(
                    x.Value, levelOfAssurance
                )).FirstOrDefault();
        }
    }
}
