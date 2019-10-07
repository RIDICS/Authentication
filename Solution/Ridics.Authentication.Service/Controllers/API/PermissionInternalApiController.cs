using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authorization;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/permission")]
    [JwtAuthorize(Policy = PolicyNames.InternalApiPolicy)]
    public class PermissionInternalApiV1Controller : ApiControllerBase
    {
        private readonly PermissionManager m_permissionManager;

        public PermissionInternalApiV1Controller(
            PermissionManager permissionsManager, IdentityUserManager identityUserManager
        ) : base(identityUserManager)
        {
            m_permissionManager = permissionsManager;
        }

        [HttpGet("list")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserPermissions)]
        [ProducesResponseType(typeof(ListContract<PermissionContract>), StatusCodes.Status200OK)]
        public ActionResult ListPermissions([FromQuery] int start = 0, [FromQuery] int count = DefaultListCount, [FromQuery] string search = null, [FromQuery] bool fetchRoles = false)
        {
            if (count > MaxListCount)
            {
                count = MaxListCount;
            }

            var permissionsResult = m_permissionManager.GetPermissions(start, count, search, fetchRoles);
            var permissionCountResult = m_permissionManager.GetPermissionsCount(search);

            if (permissionsResult.HasError)
            {
                return Error(permissionsResult.Error);
            }

            if (permissionCountResult.HasError)
            {
                return Error(permissionCountResult.Error);
            }

            FilterAuthServiceRoles(permissionsResult.Result);

            var permissionContracts = Mapper.Map<List<PermissionContract>>(permissionsResult.Result);

            var contractList = new ListContract<PermissionContract>
            {
                Items = permissionContracts,
                ItemsCount = permissionCountResult.Result,
            };

            Response.Headers.Add(HeaderStart, start.ToString());
            Response.Headers.Add(HeaderCount, count.ToString());

            return Json(contractList);
        }

        private void FilterAuthServiceRoles(List<PermissionModel> permissions)
        {
            foreach (var permission in permissions)
            {
                permission.Roles = permission.Roles.Where(x => !x.AuthenticationServiceOnly).ToList(); //filter auth only roles
            }
        }

        [HttpGet("allpermissions")]
        [JwtAuthorize]
        [ProducesResponseType(typeof(IList<PermissionContractBase>), StatusCodes.Status200OK)]
        public IActionResult AllPermissions([FromQuery] string search = null)
        {
            var permissionsResult = m_permissionManager.GetAllPermissions(search);

            if (permissionsResult.HasError)
            {
                return Error(permissionsResult.Error);
            }

            FilterAuthServiceRoles(permissionsResult.Result);

            var permissionContracts = Mapper.Map<IList<PermissionContractBase>>(permissionsResult.Result);

            return Json(permissionContracts);
        }

        [HttpPost("create")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserPermissions)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public ActionResult Create([Required] [FromBody] PermissionContractBase permissionContract)
        {
            var permissionModel = Mapper.Map<PermissionModel>(permissionContract);

            var result = m_permissionManager.CreatePermission(permissionModel);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok(result.Result);
        }

        [HttpGet("check")]
        [JwtAuthorize]
        public ActionResult CheckUserHasPermission([Required] [FromQuery] int? userId, [Required] [FromQuery] string permissionName)
        {
            if (userId == null)
            {
                return BadRequest();
            }

            var dataResult = m_permissionManager.CheckUserHasPermission(userId.Value, permissionName);

            if (dataResult.HasError)
            {
                return Error(dataResult.Error);
            }

            return Json(dataResult.Result);
        }

        [HttpGet("{id}")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserPermissions)]
        public ActionResult Get([Required] [FromRoute] int id, [FromQuery] bool fetchRoles = false)
        {
            var result = m_permissionManager.FindPermissionById(id, fetchRoles);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            var permissionContract = Mapper.Map<PermissionContract>(result.Result);

            return Json(permissionContract);
        }

        [HttpPut("{id}/edit")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserPermissions)]
        public ActionResult Edit([Required] [FromRoute] int id,
            [Required] [FromBody] PermissionContractBase permissionContract)
        {
            var permissionModel = Mapper.Map<PermissionModel>(permissionContract);

            var result = m_permissionManager.UpdatePermission(id, permissionModel);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }

        [HttpDelete("{id}/delete")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserPermissions)]
        public ActionResult Delete([Required] [FromRoute] int id)
        {
            var result = m_permissionManager.DeletePermissionWithId(id);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }

        [HttpPost("{id}/roles")]
        [JwtAuthorize(Policy = PermissionNames.AssignPermissionsToRoles)]
        public IActionResult AssignRolesToPermissions([Required] [FromRoute] int id, [Required] [FromBody] List<int> selectedRoles)
        {
            var result = m_permissionManager.AssignRolesToPermissions(id, selectedRoles, false);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }

        [HttpPut("ensure")]
        [JwtAuthorize]
        public IActionResult EnsurePermissionsExist([Required] [FromBody] EnsurePermissionsContract data)
        {
            var permissionsModel = Mapper.Map<IList<PermissionInfoModel>>(data.Permissions);
            var result = m_permissionManager.EnsurePermissionsExist(permissionsModel, data.NewAssignToRoleName);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }
    }
}