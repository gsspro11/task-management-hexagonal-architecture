using Microsoft.AspNetCore.Mvc;
using TaskManagement.HexagonalArchitecture.Application.Commom.Notifications.v1;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
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
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status422UnprocessableEntity)]
    public class UserController(IUserService _userService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetAsync([FromQuery] string email)
        {
            var result = await _userService.GetAsync(email);

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
        public async Task<ActionResult> CreateAsync([FromBody] UserRequest request)
        {
            var result = await _userService.CreateAsync(request.FirstName, request.LastName, request.Email, request.Password);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Created("api/v1/user", result.Value.Id);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync(string email, [FromBody] UserRequest request)
        {
            var result = await _userService.UpdateAsync(email, request.FirstName, request.LastName, request.Email);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value.Id);
        }
    }
}
