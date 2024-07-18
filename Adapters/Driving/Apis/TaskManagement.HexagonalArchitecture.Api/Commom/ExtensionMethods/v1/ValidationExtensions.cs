using FluentValidation.AspNetCore;
using FluentValidation;
using TaskManagement.HexagonalArchitecture.Api.Controllers.Users;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using System.Text.Json.Serialization;

namespace TaskManagement.HexagonalArchitecture.Api.Commom.ExtensionMethods.v1
{
    public static partial class ValidationExtensions
    {
        public static void AddValidation(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => JsonSerializer.Deserialize<CustomError>(e.ErrorMessage));

                        return new BadRequestObjectResult(errors);
                    };
                });

            services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining<UserRequestValidator>();

            services.AddTransient<IValidatorInterceptor, CustomErrorModelInterceptor>();
        }
    }
}
