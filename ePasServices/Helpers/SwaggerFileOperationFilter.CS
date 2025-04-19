using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ePasServices.Helpers
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody != null &&
                operation.RequestBody.Content.ContainsKey("multipart/form-data"))
            {
                var content = operation.RequestBody.Content["multipart/form-data"];
                content.Schema.Properties.TryAdd("file", new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                });
            }
        }
    }
}
