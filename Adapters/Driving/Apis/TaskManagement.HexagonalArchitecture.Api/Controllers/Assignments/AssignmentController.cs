using Microsoft.AspNetCore.Mvc;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;

namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Assignments
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
    [Route("api/v{version:apiVersion}/assignment")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status422UnprocessableEntity)]
    public class AssignmentController(IAssignmentService assignmentService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetAsync([FromQuery] string email)
        {
            var result = await assignmentService.GetAsync(email);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new
            {
                result.Value.Id,
                result.Value.Title,
                result.Value.Description,
                result.Value.DueDate,
                result.Value.Priority,
                result.Value.Status,
                result.Value.CreatedDate,
                result.Value.UpdatedDate
            });
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] AssignmentRequest request)
        {
            var result = await assignmentService.CreateAsync(request.Title, request.Description, request.DueDate, request.Priority, request.Status);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Created("api/v1/assignment", result.Value.Id);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync(string email, [FromBody] AssignmentRequest request)
        {
            var result = await assignmentService.UpdateAsync(request.Title, request.Description, request.DueDate, request.Priority, request.Status);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value.Id);
        }
    }
}
