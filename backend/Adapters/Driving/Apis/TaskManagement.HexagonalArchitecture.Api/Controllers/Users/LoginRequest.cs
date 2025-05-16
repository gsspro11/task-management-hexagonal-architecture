using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using TaskManagement.HexagonalArchitecture.Api.Attributes;

namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Users
{
    public class LoginRequest
    {
        [SwaggerSchema(Title = "E-mail")]
        [SwaggerSchemaExample("ben.parker@email.com")]
        public required string UserName { get; set; }

        [PasswordPropertyText]
        [SwaggerSchemaExample("P@ss123")]
        [SwaggerSchema(Title = "Password")]
        public required string Password { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("The field UserName is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("The field Password is required.");
        }
    }
}
