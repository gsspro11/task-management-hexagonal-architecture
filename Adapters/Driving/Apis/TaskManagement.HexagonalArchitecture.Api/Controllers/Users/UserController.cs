using Microsoft.AspNetCore.Mvc;
using TaskManagement.HexagonalArchitecture.Application.Commom.Notifications.v1;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;

namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Users
{
    /// <summary>
    /// Users Management
    /// </summary>
    /// <response code="400">Field validation messages</response>
    /// <response code="422">Business messages</response>
    /// <response code="500">Coding and server errors</response>
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/user")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status422UnprocessableEntity)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] UserRequest request)
        {
            var result = await _userService.Register(request.FirstName, request.LastName, request.Email, request.Password);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Created("api/v1/user", result.Value.Id);
        }
    }
}
