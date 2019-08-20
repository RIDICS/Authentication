using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.DataContracts.User;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Helpers;
using Ridics.Core.Structures;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/role")]
    [JwtAuthorize(Policy = PolicyNames.InternalApiPolicy)]
    public class RoleInternalApiV1Controller : ApiControllerBase
    {
        private readonly RoleManager m_rolesManager;
        private readonly UserManager m_usersManager;
        private readonly UserHelper m_userHelper;

        public RoleInternalApiV1Controller(RoleManager rolesManager, UserManager usersManager, UserHelper userHelper, IdentityUserManager identityUserManager) : base(identityUserManager)
        {
            m_rolesManager = rolesManager;
            m_usersManager = usersManager;
            m_userHelper = userHelper;
        }

        [HttpGet("list")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserRoles)]
        [ProducesResponseType(typeof(ListContract<RoleContract>), StatusCodes.Status200OK)]
        public IActionResult ListRoles([FromQuery] int start = 0, [FromQuery] int count = DefaultListCount, [FromQuery] string search = null)
        {
            if (count > MaxListCount)
            {
                count = MaxListCount;
            }

            var rolesCountResult = m_rolesManager.GetNonAuthenticationServiceRolesCount(search);
            var rolesResult = m_rolesManager.GetNonAuthenticationServiceRoles(start, count, search);

            if (rolesResult.HasError)
            {
                return Error(rolesResult.Error);
            }

            if (rolesCountResult.HasError)
            {
                return Error(rolesCountResult.Error);
            }

            var roleViewModels = Mapper.Map<IList<RoleContract>>(rolesResult.Result);

            var contractList = new ListContract<RoleContract>
            {
                Items = roleViewModels,
                ItemsCount = rolesCountResult.Result,
            };

            Response.Headers.Add(HeaderStart, start.ToString());
            Response.Headers.Add(HeaderCount, count.ToString());

            return Json(contractList);
        }

        [HttpGet("allroles")]
        [JwtAuthorize]
        [ProducesResponseType(typeof(IList<RoleContract>), StatusCodes.Status200OK)]
        public IActionResult AllRoles([FromQuery] int start = 0, [FromQuery] int count = DefaultListCount)
        {
            if (count > MaxListCount)
            {
                count = MaxListCount;
            }

            //TODO apply paging
            var rolesResult = m_rolesManager.GetAllNonAuthenticationServiceRoles();

            if (rolesResult.HasError)
            {
                return Error(rolesResult.Error);
            }

            var roleContracts = Mapper.Map<IList<RoleContract>>(rolesResult.Result);

            return Json(roleContracts);
        }

        [HttpPost("create")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserRoles)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public ActionResult Create([FromBody] [Required] RoleContract roleContract)
        {
            if (ModelState.IsValid)
            {
                var roleModel = Mapper.Map<RoleModel>(roleContract);

                var result = m_rolesManager.CreateRole(roleModel);

                if (result.HasError)
                {
                    return Error(result.Error);
                }

                return Ok(result.Result);
            }

            return BadRequest();
        }

        [HttpGet("{id}")]
        [JwtAuthorize]
        [ProducesResponseType(typeof(RoleContract), StatusCodes.Status200OK)]
        public ActionResult Get([Required] [FromRoute] int id)
        {

            var result = m_rolesManager.FindRoleById(id);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            var roleContract = Mapper.Map<RoleContract>(result.Result);

            return Json(roleContract);
        }


        [HttpPut("{id}/edit")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserRoles)]
        public ActionResult Edit([Required] [FromRoute] int id, [Required] [FromBody] RoleContract roleContract)
        {
            var roleModel = Mapper.Map<RoleModel>(roleContract);

            var result = m_rolesManager.UpdateRole(id, roleModel);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }

        [HttpDelete("{id}/delete")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserRoles)]
        public ActionResult Delete([Required] [FromRoute] int id)
        {
            var result = m_rolesManager.DeleteRoleWithId(id);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }

        [HttpPost("{id}/permissions")]
        [JwtAuthorize(Policy = PermissionNames.AssignPermissionsToRoles)]
        public IActionResult AssignPermissionsToRole([FromRoute] [Required] int id,
            [FromBody] [Required] IEnumerable<int> permissionIds)
        {
            var result = m_rolesManager.AssignPermissionsToRole(id, permissionIds);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }

        [HttpGet("{id}/users")]
        [JwtAuthorize(Policy = PermissionNames.ListUsers)]
        [ProducesResponseType(typeof(ListContract<UserWithRolesContract>), StatusCodes.Status200OK)]
        public IActionResult GetUsersByRole(
            [Required] [FromRoute] int id, [FromQuery] int start = 0, [FromQuery] int count = DefaultListCount, [FromQuery] string search = null)
        {
            if (count > MaxListCount)
            {
                count = MaxListCount;
            }

            var usersResult = m_usersManager.FindUsersByRole(id, start, count, search);
            var usersCountResult = m_usersManager.GetUsersByRoleCount(id, search);

            if (usersResult.HasError)
            {
                return Error(usersResult.Error);
            }

            if (usersCountResult.HasError)
            {
                return Error(usersCountResult.Error);
            }
            
            var userContracts = Mapper.Map<IList<UserWithRolesContract>>(usersResult.Result);

            Response.Headers.Add(HeaderStart, start.ToString());
            Response.Headers.Add(HeaderCount, count.ToString());
            
            var contractList = new ListContract<UserWithRolesContract>
            {
                Items = userContracts,
                ItemsCount = usersCountResult.Result,
            }; 

            return Json(contractList);
        }

        [HttpGet]
        [JwtAuthorize]
        [ProducesResponseType(typeof(RoleContract), StatusCodes.Status200OK)]
        public IActionResult GetRoleByName([Required] [FromQuery] string name)
        {
            var roleResult = m_rolesManager.FindRoleByName(name);

            if (roleResult.HasError)
            {
                return Error(roleResult.Error);
            }
            
            var role = Mapper.Map<RoleContract>(roleResult.Result);
            return Json(role);
        }
    }
}