using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ridics.Authentication.Service.Builders.Interface;
using Ridics.Authentication.Service.Factories.Interface;
using Ridics.Authentication.Service.Models.ViewModel;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Service.Controllers
{
    /// <summary>
    /// Base controller sharing logic for <seealso cref="Controller"/> (e.g. hiding from swagger)
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class AuthControllerBase<TController> : Controller
        where TController : AuthControllerBase<TController>
    {
        private ILogger<TController> m_logger;
        private IMapper m_mapper;
        private ITranslator m_translator;
        private IViewModelFactory m_viewModelFactory;
        private IViewModelBuilder m_viewModelBuilder;

        private const string ModelStateCacheKey = "modelStateCache";

        protected ILogger<TController> Logger =>
            m_logger ?? (m_logger = HttpContext.RequestServices.GetService<ILogger<TController>>());

        protected IMapper Mapper => m_mapper ?? (m_mapper = HttpContext.RequestServices.GetService<IMapper>());

        protected ITranslator Translator =>
            m_translator ?? (m_translator = HttpContext.RequestServices.GetService<ITranslator>());

        protected IViewModelFactory ViewModelFactory =>
            m_viewModelFactory ?? (m_viewModelFactory = HttpContext.RequestServices.GetService<IViewModelFactory>());

        protected IViewModelBuilder ViewModelBuilder =>
            m_viewModelBuilder ?? (m_viewModelBuilder = HttpContext.RequestServices.GetService<IViewModelBuilder>());

        protected AuthControllerBase()
        {
        }

        protected AuthControllerBase(ILogger<TController> logger, IMapper mapper, ITranslator translator)
        {
            m_logger = logger;
            m_mapper = mapper;
            m_translator = translator;
        }

        protected IList<T> GetSelectedItems<T>(IEnumerable<SelectableViewModel<T>> selectableClaimTypes)
        {
            return selectableClaimTypes?.Where(x => x.IsSelected).Select(x => x.Item).ToList();
        }

        protected void CacheModelState()
        {
            TempData[ModelStateCacheKey] = JsonConvert.SerializeObject(new SerializableError(ModelState));
        }

        //TODO investigate if action filter or handler impl should be better
        protected void LoadCachedModelState()
        {
            if (TempData.TryGetValue(ModelStateCacheKey, out var cachedModelStateObject))
            {
                var cachedErrors = JsonConvert.DeserializeObject<SerializableError>(cachedModelStateObject as string);
                var cachedModelState = new ModelStateDictionary();

                foreach (var cachedError in cachedErrors)
                {
                    var messages = JsonConvert.DeserializeObject<string[]>(cachedError.Value.ToString());
                    foreach (var message in messages)
                    {
                        cachedModelState.AddModelError(cachedError.Key, message);
                    }
                }

                ModelState.Merge(cachedModelState);
            }
        }
    }
}