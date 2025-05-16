using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using TaskManagement.HexagonalArchitecture.Api.Attributes;

namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Comments
{
    public class CommentUpdateRequest
    {
        [SwaggerSchemaExample("000000000000-0000-0000-0000-000000000000")]
        [SwaggerSchema(Title = "Id")]
        public required Guid Id { get; set; }

        [SwaggerSchemaExample("Description")]
        [SwaggerSchema(Title = "Description")]
        public required string Description { get; set; }
    }

    public class CommentUpdateRequestValidator : AbstractValidator<CommentUpdateRequest>
    {
        public CommentUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("The field AssignmentId is required.");
            
            RuleFor(x => x.Description)
                .Length(1, 20000)
                .WithMessage("The field Description must be a minimum length of '1' and maximum length of '20000'.");
        }
    }
}
