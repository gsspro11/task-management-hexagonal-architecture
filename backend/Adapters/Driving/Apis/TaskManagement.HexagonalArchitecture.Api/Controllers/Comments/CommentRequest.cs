using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using TaskManagement.HexagonalArchitecture.Api.Attributes;

namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Comments
{
    public class CommentRequest
    {
        [SwaggerSchemaExample("000000000000-0000-0000-0000-000000000000")]
        [SwaggerSchema(Title = "AssignmentId")]
        public required Guid AssignmentId { get; set; }

        [SwaggerSchemaExample("Description")]
        [SwaggerSchema(Title = "Description")]
        public required string Description { get; set; }
    }

    public class CommentRequestValidator : AbstractValidator<CommentRequest>
    {
        public CommentRequestValidator()
        {
            RuleFor(x => x.AssignmentId)
                .NotEmpty()
                .WithMessage("The field AssignmentId is required.");
            
            RuleFor(x => x.Description)
                .Length(1, 20000)
                .WithMessage("The field Description must be a minimum length of '1' and maximum length of '20000'.");
        }
    }
}
