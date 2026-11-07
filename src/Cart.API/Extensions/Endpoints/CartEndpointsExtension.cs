using MiniApi.Clients;
using MiniApi.Data;
using MiniApi.Exceptions;
using MiniApi.Models;
using MiniApi.Swagger;

namespace MiniApi.Extensions.Endpoints
{
    public static class CartEndpointsExtension
    {
        public static void MapCartEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/cart").WithTags("Cart");

            group.MapGet("/{userId}", async (string userId, CartRepository repo) =>
                Results.Ok(await GetCartOrThrow(userId, repo)))
                .WithSummary("Obtener carrito del usuario")
                .Produces<Cart>()
                .WithErrorExample(404, ErrorCodes.CRT_001, ErrorCodes.CRT_001_Msg)
                .WithErrorExample(500, ErrorCodes.CRT_005, ErrorCodes.CRT_005_Msg);

            group.MapPost("/{userId:guid}/items", async (
                Guid userId, AddCartItemRequest req, CartRepository repo, ProductsClient products) =>
            {
                RequestValidator.Validate(req);

                // Si el producto ya estaba en el carrito, se suman las unidades.
                var cart = await repo.GetAsync(userId);
                var yaEnCarrito = cart?.Items.FirstOrDefault(i => i.ProductoId == req.ProductoId)?.Cantidad ?? 0;
                var total = yaEnCarrito + req.Cantidad!.Value;

                await ValidateStock(products, req.ProductoId!.Value, total);
                await repo.SaveItemAsync(userId, req.ProductoId.Value, total);

                return Results.Ok(await repo.GetAsync(userId));
            })
            .WithSummary("Agregar producto al carrito")
            .Produces<Cart>()
            .WithErrorExample(400, ErrorCodes.CRT_004, ErrorCodes.CRT_004_Msg)
            .WithErrorExample(404, ErrorCodes.CRT_002, ErrorCodes.CRT_002_Msg)
            .WithErrorExample(422, ErrorCodes.CRT_003, ErrorCodes.CRT_003_Msg)
            .WithErrorExample(500, ErrorCodes.CRT_005, ErrorCodes.CRT_005_Msg);

            group.MapPut("/{userId}/items/{productId:guid}", async (
                string userId, Guid productId, UpdateCartItemRequest req, CartRepository repo, ProductsClient products) =>
            {
                RequestValidator.Validate(req);

                var cart = await GetCartOrThrow(userId, repo);

                if (cart.Items.All(i => i.ProductoId != productId))
                    throw NotInCart();

                await ValidateStock(products, productId, req.Cantidad!.Value);
                await repo.SaveItemAsync(cart.UsuarioId, productId, req.Cantidad.Value);

                return Results.Ok(await repo.GetAsync(cart.UsuarioId));
            })
            .WithSummary("Actualizar cantidad de un item")
            .Produces<Cart>()
            .WithErrorExample(400, ErrorCodes.CRT_004, ErrorCodes.CRT_004_Msg)
            .WithErrorExample(404, ErrorCodes.CRT_001, ErrorCodes.CRT_001_Msg)
            .WithErrorExample(404, ErrorCodes.CRT_002, ErrorCodes.CRT_002_Msg)
            .WithErrorExample(422, ErrorCodes.CRT_003, ErrorCodes.CRT_003_Msg)
            .WithErrorExample(500, ErrorCodes.CRT_005, ErrorCodes.CRT_005_Msg);

            group.MapDelete("/{userId}/items/{productId:guid}", async (string userId, Guid productId, CartRepository repo) =>
            {
                var cart = await GetCartOrThrow(userId, repo);

                if (!await repo.RemoveItemAsync(cart.UsuarioId, productId))
                    throw NotInCart();

                return Results.NoContent();
            })
            .WithSummary("Quitar un producto del carrito")
            .Produces(StatusCodes.Status204NoContent)
            .WithErrorExample(404, ErrorCodes.CRT_001, ErrorCodes.CRT_001_Msg)
            .WithErrorExample(500, ErrorCodes.CRT_005, ErrorCodes.CRT_005_Msg);

            group.MapDelete("/{userId}", async (string userId, CartRepository repo) =>
            {
                var cart = await GetCartOrThrow(userId, repo);
                await repo.DeleteAsync(cart.UsuarioId);
                return Results.NoContent();
            })
            .WithSummary("Vaciar carrito completo")
            .Produces(StatusCodes.Status204NoContent)
            .WithErrorExample(404, ErrorCodes.CRT_001, ErrorCodes.CRT_001_Msg)
            .WithErrorExample(500, ErrorCodes.CRT_005, ErrorCodes.CRT_005_Msg);
        }

        private static async Task<Cart> GetCartOrThrow(string userId, CartRepository repo)
        {
            var cart = Guid.TryParse(userId, out var guid) ? await repo.GetAsync(guid) : null;
            return cart ?? throw new NotFoundException(ErrorCodes.CRT_001, ErrorCodes.CRT_001_Msg);
        }

        private static async Task ValidateStock(ProductsClient products, Guid productId, int cantidad)
        {
            var product = await products.GetAsync(productId)
                ?? throw new NotFoundException(ErrorCodes.CRT_002, ErrorCodes.CRT_002_Msg);

            if (product.Stock < cantidad)
                throw new BusinessRuleException(
                    ErrorCodes.CRT_003,
                    $"Stock insuficiente. Disponible: {product.Stock}, solicitado: {cantidad}.",
                    StatusCodes.Status422UnprocessableEntity,
                    "No se puede procesar la solicitud.");
        }

        private static NotFoundException NotInCart() =>
            new(ErrorCodes.CRT_002, "El producto no está en el carrito.");
    }
}