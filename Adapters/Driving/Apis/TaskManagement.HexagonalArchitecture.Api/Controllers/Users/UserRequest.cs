using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TaskManagement.HexagonalArchitecture.Api.Attributes;

namespace TaskManagement.HexagonalArchitecture.Api.Controllers.Users
{
    public class UserRequest
    {
        [Length(1, 256)]
        [SwaggerSchemaExample("Ben")]
        [SwaggerSchema(Title = "First Name")]
        [Required(ErrorMessage = "The field FirstName is required.")]
        public required string FirstName { get; set; }

        [Length(1, 256)]
        [SwaggerSchemaExample("Parker")]
        [SwaggerSchema(Title = "Last Name")]
        [Required(ErrorMessage = "The field LastName is required.")]
        public required string LastName { get; set; }

        [EmailAddress]
        [Length(1, 256)]
        [SwaggerSchema(Title = "E-mail")]
        [SwaggerSchemaExample("ben.parker@email.com")]
        [Required(ErrorMessage = "The field E-mail is required.")]
        public required string Email { get; set; }

        [Length(6, 250)]
        [PasswordPropertyText]
        [SwaggerSchemaExample("P@ss123")]
        [SwaggerSchema(Title = "Password")]
        [Required(ErrorMessage = "The field Password is required.")]
        public required string Password { get; set; }
    }

    public class UserRequestValidator : AbstractValidator<UserRequest>
    {
        public UserRequestValidator()
        {
            RuleFor(x => x.Password)
                .Matches(@"[A-Z]+")
                .WithMessage("The field Password must contains 1 upper case character.")
                .Matches(@"(?=.*[}{,.^?~=+\-_\/*\-@!#$%&+.\|]).{6,}")
                .WithMessage("The field Password must contains 1 special character at least.");
        }
    }
}
