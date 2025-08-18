using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TaskManagement.HexagonalArchitecture.Api.Attributes;

namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Users
{
    public class UserRequest
    {
        [SwaggerSchemaExample("Ben")]
        [SwaggerSchema(Title = "First Name")]
        public required string FirstName { get; set; }

        [SwaggerSchemaExample("Parker")]
        [SwaggerSchema(Title = "Last Name")]
        public required string LastName { get; set; }

        [SwaggerSchema(Title = "E-mail")]
        [SwaggerSchemaExample("ben.parker@email.com")]
        public required string Email { get; set; }

        [PasswordPropertyText]
        [SwaggerSchemaExample("P@ss123")]
        [SwaggerSchema(Title = "Password")]
        public required string Password { get; set; }
    }

    public class UserRequestValidator : AbstractValidator<UserRequest>
    {
        public UserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .Length(1, 256)
                .WithMessage("The field FirstName must be a minimum length of '1' and maximum length of '256'.");

            RuleFor(x => x.LastName)
                .Length(1, 256)
                .WithMessage("The field LastName must be a minimum length of '1' and maximum length of '256'.");

            RuleFor(x => x.Email)
                .Length(1, 256)
                .WithMessage("The field Email must be a minimum length of '1' and maximum length of '256'.")
                .EmailAddress()
                .WithMessage("The Email field is not a valid e-mail address.");

            RuleFor(x => x.Password)
                .Length(6, 250)
                .WithMessage("The field Password must be a minimum length of '6' and maximum length of '250'.")
                .Matches(@"[A-Z]+")
                .WithMessage("The field Password must contains 1 upper case character.")
                .Matches(@"(?=.*[}{,.^?~=+\-_\/*\-@!#$%&+.\|]).{6,}")
                .WithMessage("The field Password must contains 1 special character at least.");
        }
    }
}
