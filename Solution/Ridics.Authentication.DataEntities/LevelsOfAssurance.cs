using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities
{
    public class LevelsOfAssurance
    {
        //Minimum level of assurance of user data to be verified for external service api (greater than or equal)
        public const LevelOfAssuranceEnum UserDataMinLoaToBeActivatedForExternalService = LevelOfAssuranceEnum.Substantial;

        //Minimum level of assurance of user data to be unique in database (greater than)
        public const LevelOfAssuranceEnum UserDataLoaToBeGreaterThanForUniqueValidation = LevelOfAssuranceEnum.Low;

        //Level of assurance set after user data verification
        public const LevelOfAssuranceEnum UserDataLoaAfterVerification = LevelOfAssuranceEnum.High;

        //Level of assurance set after user data invalidation   
        public const LevelOfAssuranceEnum UserDataLoaAfterInvalidation = LevelOfAssuranceEnum.Low;

        //Level of assurance set after contact confirmation
        public const LevelOfAssuranceEnum ContactLoaAfterConfirmation = LevelOfAssuranceEnum.High;

        //Level of assurance set after contact change
        public const LevelOfAssuranceEnum ContactLoaAfterChange = LevelOfAssuranceEnum.Low;

        //Minimum level of assurance to get user by confirmed email (greater than or equal)
        public const LevelOfAssuranceEnum ContactMinLoaToGetUserByConfirmedEmail = LevelOfAssuranceEnum.Substantial;

        //Minimum level of assurance of contacts to be unique in database (greater than)
        public const LevelOfAssuranceEnum ContactLoaToBeGreaterThanForUniqueValidation = LevelOfAssuranceEnum.Low;

        //Minimum level of assurance of contact to be confirmed, mainly for identity (greater than or equal)
        public const LevelOfAssuranceEnum ContactMinLoaToBeConfirmed = LevelOfAssuranceEnum.Substantial;

        //Minimum level of assurance of user data to get user (greater than or equal)
        public const LevelOfAssuranceEnum UserDataMinLoaToGetUserByVerifiedUserData = LevelOfAssuranceEnum.Substantial;
    }
}