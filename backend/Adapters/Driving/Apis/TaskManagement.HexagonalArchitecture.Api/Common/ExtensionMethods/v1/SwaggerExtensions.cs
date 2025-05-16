using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TaskManagement.HexagonalArchitecture.Api.Attributes;

namespace TaskManagement.HexagonalArchitecture.Api.Common.ExtensionMethods.v1
{
    public static class SwaggerExtensions
    {
        private static readonly OpenApiSecurityScheme SecurityScheme = new()
        {
            Name = "JWT Authentication",
            Description = "Enter your JWT token in this field",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT"
        };
        
        private static readonly OpenApiSecurityRequirement SecurityRequirement = new()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                []
            }
        };
        
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerExamples();
            
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SchemaFilter<SwaggerSchemaExampleFilter>();
                
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, SecurityScheme);
                c.AddSecurityRequirement(SecurityRequirement);
            });
        }
    }
}