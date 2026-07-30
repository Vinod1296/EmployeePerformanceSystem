using EmployeePerformance.Application.DTOs.UserManagement;
using EmployeePerformance.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeePerformance.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UsersController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(EmployeePerformance.Application.DTOs.Common.PagedResultDto<UserListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<EmployeePerformance.Application.DTOs.Common.PagedResultDto<UserListDto>>> GetUsers([FromQuery] UserQueryParametersDto queryParameters)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var response = await _userManagementService.GetUsersAsync(queryParameters);
            return Ok(response);
        }

        [HttpGet("{userId:int}")]
        [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDetailsDto>> GetUserById(int userId)
        {
            var response = await _userManagementService.GetUserByIdAsync(userId);
            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPut("{userId:int}/role")]
        [ProducesResponseType(typeof(UpdateUserRoleResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdateUserRoleResponseDto>> UpdateRole(int userId, [FromBody] UpdateUserRoleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var authenticatedUserId = GetAuthenticatedUserId();
            if (authenticatedUserId is null)
            {
                return Unauthorized();
            }

            var response = await _userManagementService.UpdateUserRoleAsync(userId, request.Role, authenticatedUserId.Value);
            return Ok(response);
        }

        [HttpPut("{userId:int}/status")]
        [ProducesResponseType(typeof(UpdateUserStatusResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        // Existing JWTs remain valid until they expire; this endpoint only updates Users.IsActive.
        public async Task<ActionResult<UpdateUserStatusResponseDto>> UpdateStatus(int userId, [FromBody] UpdateUserStatusRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var authenticatedUserId = GetAuthenticatedUserId();
            if (authenticatedUserId is null)
            {
                return Unauthorized();
            }

            var response = await _userManagementService.UpdateUserStatusAsync(userId, request.IsActive!.Value, authenticatedUserId.Value);
            return Ok(response);
        }

        private int? GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId");
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
