using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace TaskManagement.HexagonalArchitecture.Api.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Struct |
        AttributeTargets.Parameter |
        AttributeTargets.Property |
        AttributeTargets.Enum,
        AllowMultiple = false)]
    public class SwaggerSchemaExampleAttribute : Attribute
    {
        public string Example { get; set; }

        public SwaggerSchemaExampleAttribute(string example)
        {
            if ("UUID".Equals(example))
            {
                Example = Guid.NewGuid().ToString();
            }
            else if ("DateOnly".Equals(example))
            {

                DateTime amanha = DateTime.Now.AddDays(1);
                Example = amanha.ToString("yyyy-MM-dd");
            }
            else
            {
                Example = example;
            }
        }
    }
    public class SwaggerSchemaExampleFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.MemberInfo != null)
            {
                var schemaAttribute = context.MemberInfo.GetCustomAttributes<SwaggerSchemaExampleAttribute>()
               .FirstOrDefault();
                if (schemaAttribute != null)
                    ApplySchemaAttribute(schema, schemaAttribute);
            }
        }
        private void ApplySchemaAttribute(OpenApiSchema schema, SwaggerSchemaExampleAttribute schemaAttribute)
        {
            if (schemaAttribute.Example != null)
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiString(schemaAttribute.Example);
            }
        }
    }
}
