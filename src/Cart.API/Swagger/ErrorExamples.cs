using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MiniApi.Swagger;

public record ErrorExample(int Status, string Code, string Message);

public static class ErrorExampleExtensions
{
    // Uso: .WithErrorExample(404, ErrorCodes.PRD_001, ErrorCodes.PRD_001_Msg)
    public static RouteHandlerBuilder WithErrorExample(
        this RouteHandlerBuilder builder, int status, string code, string message) =>
        builder.WithMetadata(new ErrorExample(status, code, message));
}

// Convierte cada ErrorExample del endpoint en una respuesta documentada en Swagger.
public class ErrorExamplesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var byStatus = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .OfType<ErrorExample>()
            .GroupBy(e => e.Status);

        operation.Responses ??= new OpenApiResponses();

        // Un status puede tener varios códigos: USR-004 y USR-005 son ambos 403.
        foreach (var group in byStatus)
        {
            var mediaType = new OpenApiMediaType { Examples = new Dictionary<string, IOpenApiExample>() };

            foreach (var example in group)
            {
                mediaType.Examples[example.Code] = new OpenApiExample
                {
                    Summary = example.Message,
                    Value = new JsonObject
                    {
                        ["status"] = example.Status,
                        ["instance"] = "/" + context.ApiDescription.RelativePath,
                        ["errorCode"] = example.Code,
                        ["errorMessage"] = example.Message,
                        ["correlationId"] = "9f3c2c1e-0000-0000-0000-000000000000"
                    }
                };
            }

            operation.Responses[group.Key.ToString()] = new OpenApiResponse
            {
                Description = string.Join(", ", group.Select(e => e.Code)),
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/problem+json"] = mediaType
                }
            };
        }
    }
}