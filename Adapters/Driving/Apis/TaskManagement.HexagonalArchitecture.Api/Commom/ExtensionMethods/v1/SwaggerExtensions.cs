using Swashbuckle.AspNetCore.Filters;
using TaskManagement.HexagonalArchitecture.Api.Attributes;

namespace TaskManagement.HexagonalArchitecture.Api.Commom.ExtensionMethods.v1
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerExamples();
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SchemaFilter<SwaggerSchemaExampleFilter>();

            });
        }
    }
}
