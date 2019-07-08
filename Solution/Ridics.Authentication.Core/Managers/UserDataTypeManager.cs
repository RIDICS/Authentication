using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Shared;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.Core.Managers
{
    public class UserDataTypeManager : ManagerBase, IUserDataTypeManager
    {
        private readonly UserDataTypeUoW m_userDataTypeUoW;

        public UserDataTypeManager(UserDataTypeUoW userDataTypeUoW, ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_userDataTypeUoW = userDataTypeUoW;
        }

        public DataResult<UserDataTypeModel> GetDataTypeByValue(string dataTypeValue)
        {
            try
            {
                var dataType = m_userDataTypeUoW.FindUserDataTypeByValue(dataTypeValue);
                var viewModel = m_mapper.Map<UserDataTypeModel>(dataType);
                return Success(viewModel);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserDataTypeModel>(m_translator.Translate("invalid-user-data-type-value"), DataResultErrorCode.UserDataInvalidType);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserDataTypeModel>(e.Message);
            }
        }

        public DataResult<List<UserDataTypeModel>> GetAllUserDataTypes()
        {
            try
            {
                var userDataTypes = m_userDataTypeUoW.GetAllUserDataTypes();
                var viewModelList = m_mapper.Map<List<UserDataTypeModel>>(userDataTypes);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<UserDataTypeModel>>(e.Message);
            }
        }
    }
}