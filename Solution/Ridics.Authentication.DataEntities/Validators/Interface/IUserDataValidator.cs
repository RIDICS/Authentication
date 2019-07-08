using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Validators.Interface
{
    public interface IUserDataValidator
    {
        string Type { get; }

        void Validate(UserDataEntity userData, ValidationOptions validationOptions);
    }
}