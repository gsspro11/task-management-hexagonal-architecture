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
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status422UnprocessableEntity)]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        [Route("{userId}")]
        public async Task<ActionResult> GetAsync([FromRoute] Guid userId)
        {
            var result = await userService.GetAsync(userId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new
            {
                result.Value.Id,
                result.Value.FirstName,
                result.Value.LastName,
                result.Value.Email,
                result.Value.UserName,
                result.Value.CreatedDate,
                result.Value.UpdatedDate
            });
        }
        
        [HttpGet]
        public async Task<ActionResult> GetByEmailAsync([FromQuery] string email)
        {
            var result = await userService.GetByEmailAsync(email);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new
            {
                result.Value.Id,
                result.Value.FirstName,
                result.Value.LastName,
                result.Value.Email,
                result.Value.UserName,
                result.Value.CreatedDate,
                result.Value.UpdatedDate
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(UserRequest request)
        {
            var result = await userService.CreateAsync(request.FirstName, request.LastName, request.Email, request.Password);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Created("api/v1/user", result.Value.Id);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync(Guid userId, [FromBody] UserRequest request)
        {
            var result = await userService.UpdateAsync(userId, request.FirstName, request.LastName, request.Email);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value.Id);
        }
        
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(string email)
        {
            var result = await userService.DeleteAsync(email);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value.Id);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> LoginAsync(LoginRequest request)
        {
            var result = await userService.LoginAsync(request.UserName, request.Password);

            if (result.IsFailure)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }
    }
}
