using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authorization;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/fileResource")]
    [JwtAuthorize(Policy = PolicyNames.InternalApiPolicy)]
    public class FileResourceInternalApiV1Controller : ApiControllerBase
    {
        private readonly FileResourceManager m_fileResourceModelManager;
        private readonly IFileResourceManager m_fileResourceManager;

        public FileResourceInternalApiV1Controller(
            FileResourceManager fileResourceModelManager,
            IFileResourceManager fileResourceManager,
            IdentityUserManager identityUserManager
        ) : base(identityUserManager)
        {
            m_fileResourceModelManager = fileResourceModelManager;
            m_fileResourceManager = fileResourceManager;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        public IActionResult Get([Required] int id)
        {
            var fileResourceModelResult = m_fileResourceModelManager.GetFileResource(id);

            if (fileResourceModelResult.HasError)
            {
                return Error(fileResourceModelResult.Error);
            }

            var fileResourceModel = fileResourceModelResult.Result;

            if (fileResourceModel == null)
            {
                return NotFound(new
                {
                    Id = id,
                    Error = "fileResource not found"
                });
            }

            var fileResourceStream = m_fileResourceManager.GetReadStream(fileResourceModel);
            if (fileResourceStream == null)
            {
                return NotFound();
            }

            Response.Headers.Add(HeaderFileName, m_fileResourceManager.ResolveName(fileResourceModel));
            Response.Headers.Add(HeaderGuid, fileResourceModel.Guid.ToString());
            Response.Headers.Add(HeaderType, fileResourceModel.Type.ToString());
            Response.Headers.Add(HeaderFileExtension, fileResourceModel.FileExtension);

            return new FileStreamResult(fileResourceStream, MediaTypeNames.Application.Octet);
        }
    }
}
