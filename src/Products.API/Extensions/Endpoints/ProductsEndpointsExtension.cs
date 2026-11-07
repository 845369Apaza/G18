using MiniApi.Clients;
using MiniApi.Data;
using MiniApi.Exceptions;
using MiniApi.Models;
using MiniApi.Swagger;

namespace MiniApi.Extensions.Endpoints
{
    public static class ProductsEndpointsExtension
    {
        public static void MapProductEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/products").WithTags("Products");

            group.MapGet("/", async (string? categoria, string? nombre, ProductRepository repo) =>
                Results.Ok(await repo.GetAllAsync(categoria, nombre)))
                .WithSummary("Listar productos")
                .WithDescription("Admite los filtros opcionales ?categoria= y ?nombre=.")
                .Produces<IEnumerable<Product>>()
                .WithErrorExample(500, ErrorCodes.PRD_005, ErrorCodes.PRD_005_Msg);

            // El id llega como string: el contrato pide 404 PRD-001 para /api/products/99.
            group.MapGet("/{id}", async (string id, ProductRepository repo) =>
            {
                var product = await repo.GetByIdAsync(ParseId(id)) ?? throw NotFound();
                return Results.Ok(product);
            })
            .WithSummary("Obtener producto por ID")
            .Produces<Product>()
            .WithErrorExample(404, ErrorCodes.PRD_001, ErrorCodes.PRD_001_Msg)
            .WithErrorExample(500, ErrorCodes.PRD_005, ErrorCodes.PRD_005_Msg);

            group.MapPost("/", async (ProductRequest req, ProductRepository repo) =>
            {
                RequestValidator.Validate(req);

                if (await repo.ExistsAsync(req.Nombre, req.Categoria))
                    throw new BusinessRuleException(
                        ErrorCodes.PRD_003,
                        $"Ya existe un producto con ese nombre en la categoría '{req.Categoria}'.",
                        StatusCodes.Status409Conflict,
                        "Ya existe un recurso con esos datos.");

                var product = await repo.CreateAsync(req);
                return Results.Created($"/api/products/{product.Id}", product);
            })
            .WithSummary("Crear producto")
            .Produces<Product>(StatusCodes.Status201Created)
            .WithErrorExample(400, ErrorCodes.PRD_002, ErrorCodes.PRD_002_Msg)
            .WithErrorExample(409, ErrorCodes.PRD_003, ErrorCodes.PRD_003_Msg)
            .WithErrorExample(500, ErrorCodes.PRD_005, ErrorCodes.PRD_005_Msg);

            group.MapPut("/{id}", async (string id, ProductRequest req, ProductRepository repo) =>
            {
                var guid = ParseId(id);
                RequestValidator.Validate(req);

                if (!await repo.UpdateAsync(guid, req))
                    throw NotFound();

                return Results.Ok(await repo.GetByIdAsync(guid));
            })
            .WithSummary("Actualizar producto")
            .Produces<Product>()
            .WithErrorExample(400, ErrorCodes.PRD_002, ErrorCodes.PRD_002_Msg)
            .WithErrorExample(404, ErrorCodes.PRD_001, ErrorCodes.PRD_001_Msg)
            .WithErrorExample(500, ErrorCodes.PRD_005, ErrorCodes.PRD_005_Msg);

            group.MapDelete("/{id}", async (string id, ProductRepository repo, OrdersClient orders) =>
            {
                var guid = ParseId(id);

                if (await repo.GetByIdAsync(guid) is null)
                    throw NotFound();

                if (await orders.HasActiveOrdersAsync(guid))
                    throw new BusinessRuleException(
                        ErrorCodes.PRD_004,
                        ErrorCodes.PRD_004_Msg,
                        StatusCodes.Status409Conflict,
                        "No se puede eliminar el recurso.");

                await repo.DeleteAsync(guid);
                return Results.NoContent();
            })
            .WithSummary("Eliminar producto")
            .Produces(StatusCodes.Status204NoContent)
            .WithErrorExample(404, ErrorCodes.PRD_001, ErrorCodes.PRD_001_Msg)
            .WithErrorExample(409, ErrorCodes.PRD_004, ErrorCodes.PRD_004_Msg)
            .WithErrorExample(500, ErrorCodes.PRD_005, ErrorCodes.PRD_005_Msg);
        }

        private static Guid ParseId(string id) =>
            Guid.TryParse(id, out var guid) ? guid : throw NotFound();

        private static NotFoundException NotFound() =>
            new(ErrorCodes.PRD_001, ErrorCodes.PRD_001_Msg);
    }
}
