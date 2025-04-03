using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using TaskManagement.HexagonalArchitecture.Api.Attributes;
using TaskManagement.HexagonalArchitecture.Domain.Enums;

namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Assignments
{
    public class AssignmentRequest
    {
        [SwaggerSchemaExample("Title")]
        [SwaggerSchema(Title = "Title")]
        public required string Title { get; set; }

        [SwaggerSchemaExample("Description")]
        [SwaggerSchema(Title = "Description")]
        public required string Description { get; set; }

        [SwaggerSchema(Title = "DueDate")]
        [SwaggerSchemaExample("03/04/2025")]
        public required DateTime DueDate { get; set; }
        
        [SwaggerSchema(Title = "Priority")]
        [SwaggerSchemaExample("1")]
        public required int Priority { get; set; }

        [SwaggerSchema(Title = "Status")]
        [SwaggerSchemaExample("N")]
        public required AssignmentStatus Status { get; set; }
    }

    public class AssignmentRequestValidator : AbstractValidator<AssignmentRequest>
    {
        public AssignmentRequestValidator()
        {
            RuleFor(x => x.Title)
                .Length(1, 256)
                .WithMessage("The field Title must be a minimum length of '1' and maximum length of '256'.");

            RuleFor(x => x.Description)
                .Length(1, 20000)
                .WithMessage("The field LastName must be a minimum length of '1' and maximum length of '20000'.");

            RuleFor(x => x.DueDate)
                .Must(x => x != DateTime.MinValue)
                .WithMessage("The field DueDate must be a valid date.");

            RuleFor(x => x.Priority)
                .GreaterThan(1)
                .WithMessage("The field Priority must be a minimum value of '1'.");
            
            RuleFor(x => x.Status)
                .Must(i => Enum.IsDefined(typeof(AssignmentStatus), i))
                .WithMessage("The field Status must be a valid assignment status.");
        }
    }
}
