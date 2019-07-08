using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Validators.Config
{
    public class ValidatorConfig
    {
        /// <summary>
        /// Dictionary that gets loaded from appsettings config file.
        /// Keys are names of validators and values are lists of user data types that the specific validator should validate.
        /// </summary>
        public IDictionary<string, IList<string>> ValidatorsToValidatedDictionary { get; set; }
    }
}