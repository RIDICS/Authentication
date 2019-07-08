using System.Linq;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Modules.Shared;
using Ridics.Authentication.Service.Models;

namespace Ridics.Authentication.Service.DynamicModule
{
    public class DynamicModuleExternalLoginSynchronization
    {
        private readonly DynamicModuleProvider m_dynamicModuleProvider;
        private readonly DynamicModuleManager m_dynamicModuleManager;
        private readonly ExternalLoginProviderManager m_externalLoginProviderManager;
        private readonly DataSourceManager m_dataSourceManager;

        public DynamicModuleExternalLoginSynchronization(
            DynamicModuleProvider dynamicModuleProvider,
            DynamicModuleManager dynamicModuleManager,
            ExternalLoginProviderManager externalLoginProviderManager,
            DataSourceManager dataSourceManager
        )
        {
            m_dynamicModuleProvider = dynamicModuleProvider;
            m_dynamicModuleManager = dynamicModuleManager;
            m_externalLoginProviderManager = externalLoginProviderManager;
            m_dataSourceManager = dataSourceManager;
        }

        public void Synchronize()
        {
            foreach (var moduleContext in m_dynamicModuleProvider.ModuleContexts)
            {
                if (
                    moduleContext.LibModuleInfo is IExternalLoginProviderModule
                    && moduleContext.ModuleConfiguration?.Name != null
                )
                {
                    var dynamicModuleRequest = m_dynamicModuleManager.GetByName(moduleContext.ModuleConfiguration.Name);

                    var dynamicModule = dynamicModuleRequest.Result;

                    if (dynamicModule != null)
                    {
                        var moduleInfo = m_dynamicModuleProvider.ModuleContexts
                            .FirstOrDefault(x => x.LibModuleInfo.ModuleGuid == dynamicModule.ModuleGuid);

                        if (moduleInfo == null)
                        {
                            continue;
                        }

                        var externalLoginProviderRequest = m_externalLoginProviderManager.GetExternalLoginProvidersByDynamicModule(
                            dynamicModule
                        );
                        var externalLoginProvider = externalLoginProviderRequest.Result;

                        if (externalLoginProvider == null)
                        {
                            externalLoginProviderRequest = m_externalLoginProviderManager.GetExternalLoginProvidersByName(
                                moduleContext.ModuleConfiguration.Name
                            );
                            externalLoginProvider = externalLoginProviderRequest.Result;

                            if (externalLoginProvider != null)
                            {
                                m_externalLoginProviderManager.UpdateDynamicModule(externalLoginProvider, dynamicModule);
                            }
                        }

                        if (externalLoginProvider != null)
                        {
                            SynchronizeMainLogo(externalLoginProvider, dynamicModule);
                            m_externalLoginProviderManager.UpdateNames(externalLoginProvider, dynamicModule, moduleInfo.ModuleConfigType);
                        }
                        else
                        {
                            m_externalLoginProviderManager.CreateExternalLoginByDynamicModule(dynamicModule, moduleInfo.ModuleConfigType);

                            externalLoginProviderRequest = m_externalLoginProviderManager.GetExternalLoginProvidersByDynamicModule(
                                dynamicModule
                            );
                            externalLoginProvider = externalLoginProviderRequest.Result;
                        }

                        var dataSourceRequest = m_dataSourceManager.GetDataSourceByDataSource(
                            DataSourceEnumModel.ExternalLoginProvider,
                            externalLoginProvider
                        );
                        var dataSource = dataSourceRequest.Result;

                        if (dataSource == null)
                        {
                            m_dataSourceManager.AddDataSource(
                                DataSourceEnumModel.ExternalLoginProvider,
                                externalLoginProvider
                            );
                        }
                    }
                }
            }
        }

        private void SynchronizeMainLogo(ExternalLoginProviderModel externalLoginProvider, DynamicModuleModel dynamicModule)
        {
            var dynamicModuleMainLogo = dynamicModule.DynamicModuleBlobs.FirstOrDefault(
                x => x.Type == DynamicModuleBlobEnumModel.MainLogo
            );

            if (dynamicModuleMainLogo != null)
            {
                m_externalLoginProviderManager.UpdateLogo(externalLoginProvider, dynamicModuleMainLogo.File);
            }
        }
    }
}
