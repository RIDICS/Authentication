using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Comparer
{
    public class UserContactEqualityComparer  
    {
        public bool Equals(UserContactEntity x, UserContactEntity y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(null, y)) return false;
            if (ReferenceEquals(x, null)) return false;

            return Equals(x.User?.Id, y.User?.Id) && //User has to be same
                   Equals(x.Type, y.Type) && //Type has to be same
                   string.Equals(x.ConfirmCode, y.ConfirmCode) && //ConfirmCode has to be same
                   Equals(x.ConfirmCodeChangeTime, y.ConfirmCodeChangeTime) && //ConfirmCodeChangeTime has to be same
                   string.Equals(x.Value, y.Value) && //Value has to be same
                   Equals(x.LevelOfAssurance?.Id, y.LevelOfAssurance?.Id) &&  //Level of assurance has to be same
                   Equals(x.DataSource?.Id, y.DataSource?.Id); //Data source has to be same
        }
    }
}