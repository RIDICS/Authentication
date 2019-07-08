using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Core.Managers
{
    public class GrantTypeManager : ManagerBase
    {
        private readonly GrantTypeUoW m_grantTypeUoW;

        public GrantTypeManager(GrantTypeUoW grantTypeUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_grantTypeUoW = grantTypeUoW;
        }

        public DataResult<List<GrantTypeModel>> GetAllGrantTypes()
        {
            var grantTypes = m_grantTypeUoW.GetAllGrantTypes();

            if (grantTypes == null || grantTypes.Count == 0)
                return Error<List<GrantTypeModel>>();

            var viewModelList = m_mapper.Map<List<GrantTypeModel>>(grantTypes);
            return Success(viewModelList);
        }
    }
}