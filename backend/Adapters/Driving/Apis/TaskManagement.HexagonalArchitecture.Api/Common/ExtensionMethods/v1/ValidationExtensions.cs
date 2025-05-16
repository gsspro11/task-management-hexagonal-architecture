using FluentValidation.AspNetCore;
using FluentValidation;
using TaskManagement.HexagonalArchitecture.Api.Controllers.Users;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using TaskManagement.HexagonalArchitecture.Api.Common.Interceptors.v1;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TaskManagement.HexagonalArchitecture.Api.Common.ExtensionMethods.v1
{
    public static class ValidationExtensions
    {
        public static void AddValidation(this IServiceCollection services)
        {
            services.AddControllers()
                // .AddNewtonsoftJson(options =>
                //     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //     )
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
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