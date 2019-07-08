using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Comparer
{
    public class UserDataEqualityComparer
    {
        public virtual bool Equals(UserDataEntity x, UserDataEntity y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(null, y)) return false;
            if (ReferenceEquals(x, null)) return false;
            
            return Equals(x.User?.Id, y.User?.Id) && //User has to be same
                   Equals(x.UserDataType?.Id, y.UserDataType?.Id) && //DataType has to be same
                   Equals(x.ActiveTo, y.ActiveTo) && // Active to date has to be the same
                   Equals(x.VerifiedBy?.Id, y.VerifiedBy?.Id) && //Verifier has to be same
                   string.Equals(x.Value, y.Value) && //Value has to be same
                   Equals(x.LevelOfAssurance?.Id, y.LevelOfAssurance?.Id) &&  //Level of assurance has to be same
                   Equals(x.DataSource?.Id, y.DataSource?.Id); //Data source has to be same
        }
    }
}