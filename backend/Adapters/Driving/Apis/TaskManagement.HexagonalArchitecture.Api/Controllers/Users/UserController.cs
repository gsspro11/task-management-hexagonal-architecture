using Microsoft.AspNetCore.Mvc;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
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
    [Route("api/v1/user")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status500InternalServerError)]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        [Route("{userId:guid}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResponse>> GetAsync([FromRoute] Guid userId)
        {
            var result = await userService.GetAsync(userId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new UserResponse(
                result.Value.Id,
                result.Value.FirstName,
                result.Value.LastName,
                result.Value.Email,
                result.Value.UserName,
                result.Value.CreatedDate,
                result.Value.UpdatedDate
            ));
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResponse>> GetByEmailAsync([FromQuery] string email)
        {
            var result = await userService.GetByEmailAsync(email);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new UserResponse(
                result.Value.Id,
                result.Value.FirstName,
                result.Value.LastName,
                result.Value.Email,
                result.Value.UserName,
                result.Value.CreatedDate,
                result.Value.UpdatedDate
            ));
        }

        [HttpGet]
        [Route("autocomplete")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<string>>> GetByUserNameAsync([FromQuery] string userName)
        {
            var result = await userService.GetByUserNameAsync(userName);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> CreateAsync(UserRequest request)
        {
            var result =
                await userService.CreateAsync(request.FirstName, request.LastName, request.Email, request.Password);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Created("api/v1/user", result.Value.Id);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> UpdateAsync(Guid userId, [FromBody] UserRequest request)
        {
            var result = await userService.UpdateAsync(userId, request.FirstName, request.LastName, request.Email);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value.Id);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteAsync(string email)
        {
            var result = await userService.DeleteAsync(email);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value.Id);
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> LoginAsync(LoginRequest request)
        {
            var result = await userService.LoginAsync(request.UserName, request.Password);

            if (result.IsFailure)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }
    }
}