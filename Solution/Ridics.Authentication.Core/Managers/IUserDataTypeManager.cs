using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;

namespace Ridics.Authentication.Core.Managers
{
    public interface IUserDataTypeManager
    {
        DataResult<List<UserDataTypeModel>> GetAllUserDataTypes();
    }
}