using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v1/assignment")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<CustomError>), StatusCodes.Status422UnprocessableEntity)]
    public class AssignmentController(IHttpContextAccessor httpContextAccessor, IAssignmentService assignmentService) : ControllerBase
    {
        private readonly Guid _userId =
            Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        [HttpGet]
        [Route("{assignmentId:guid}")]
        public async Task<ActionResult> GetAsync([FromRoute] Guid assignmentId, CancellationToken cancellationToken)
        {
            var result = await assignmentService.GetAsync(assignmentId, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
        
        [HttpGet]
        public async Task<ActionResult> ListAsync([FromQuery] Guid userId, CancellationToken cancellationToken)
        {
            var result = await assignmentService.ListByUserIdAsync(userId, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] AssignmentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await assignmentService.CreateAsync(request.Title, request.Description, request.UserName,
                request.DueDate, request.Priority, request.Status, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Created("api/v1/assignment", result.Value.Id);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync([FromQuery] Guid id, [FromBody] AssignmentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await assignmentService.UpdateAsync(id, request.Title, request.Description, request.DueDate,
                request.Priority, request.Status, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value.Id);
        }
        
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await assignmentService.DeleteAsync(id, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value.Id);
        }
    }
}