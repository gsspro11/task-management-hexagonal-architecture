using FluentValidation.AspNetCore;
using FluentValidation;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;

namespace TaskManagement.HexagonalArchitecture.Api.Commom.ExtensionMethods.v1
{
    public class CustomErrorModelInterceptor : IValidatorInterceptor
    {
        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            return commonContext;
        }

        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext,
            ValidationResult result)
        {
            var failures = result.Errors
                .Select(error => new ValidationFailure(error.PropertyName, SerializeError(error)));

            return new ValidationResult(failures);
        }

        private static string SerializeError(ValidationFailure failure)
        {
            var error = new CustomError(failure.ErrorCode, failure.ErrorMessage);
            return JsonSerializer.Serialize(error);
        }
    }
}
