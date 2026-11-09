using MiniApi.Clients;
using MiniApi.Data;
using MiniApi.Exceptions;
using MiniApi.Models;
using MiniApi.Swagger;

namespace MiniApi.Extensions.Endpoints
{
    public static class OrdersEndpointsExtension
    {
        // Desde cada estado, a cuáles se puede pasar.
        private static readonly Dictionary<string, string[]> Transiciones = new()
        {
            ["Pendiente"] = ["Confirmada", "Cancelada"],
            ["Confirmada"] = ["Enviada", "Cancelada"],
            ["Enviada"] = ["Entregada"],
            ["Entregada"] = [],
            ["Cancelada"] = []
        };

        public static void MapOrderEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/orders").WithTags("Orders");

            group.MapGet("/", async (Guid? usuarioId, OrderRepository repo) =>
                Results.Ok(await repo.GetAllAsync(usuarioId)))
                .WithSummary("Listar órdenes")
                .WithDescription("Admite el filtro opcional ?usuarioId=.")
                .Produces<List<Order>>()
                .WithErrorExample(500, ErrorCodes.ORD_007, ErrorCodes.ORD_007_Msg);

            group.MapGet("/{id}", async (string id, OrderRepository repo) =>
                Results.Ok(await GetOrderOrThrow(id, repo)))
                .WithSummary("Obtener detalle de una orden")
                .Produces<Order>()
                .WithErrorExample(404, ErrorCodes.ORD_001, ErrorCodes.ORD_001_Msg)
                .WithErrorExample(500, ErrorCodes.ORD_007, ErrorCodes.ORD_007_Msg);

            group.MapPost("/", async (
                CreateOrderRequest req, OrderRepository repo, UsersClient users, ProductsClient products) =>
            {
                RequestValidator.Validate(req);

                // Validator no entra a las listas: cada item se valida aparte.
                foreach (var item in req.Items)
                    RequestValidator.Validate(item);

                if (!await users.ExistsAsync(req.UsuarioId!.Value))
                    throw new NotFoundException(ErrorCodes.ORD_003, ErrorCodes.ORD_003_Msg);

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = req.UsuarioId.Value,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.UtcNow
                };

                // Si el mismo producto viene repetido se unifica para validar bien el stock.
                var pedidos = req.Items.GroupBy(i => i.ProductoId!.Value);

                foreach (var pedido in pedidos)
                {
                    var cantidad = pedido.Sum(i => i.Cantidad!.Value);

                    var product = await products.GetAsync(pedido.Key)
                        ?? throw new NotFoundException(ErrorCodes.ORD_004, ErrorCodes.ORD_004_Msg);

                    if (product.Stock < cantidad)
                        throw new BusinessRuleException(
                            ErrorCodes.ORD_005,
                            $"Stock insuficiente para '{product.Nombre}'. Disponible: {product.Stock}, solicitado: {cantidad}.",
                            StatusCodes.Status422UnprocessableEntity,
                            "No se puede procesar la solicitud.");

                    order.Items.Add(new OrderItem
                    {
                        ProductoId = product.Id,
                        Cantidad = cantidad,
                        PrecioUnitario = product.Precio
                    });
                }

                order.Total = order.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

                await repo.CreateAsync(order);
                return Results.Created($"/api/orders/{order.Id}", order);
            })
            .WithSummary("Crear orden")
            .Produces<Order>(StatusCodes.Status201Created)
            .WithErrorExample(400, ErrorCodes.ORD_002, ErrorCodes.ORD_002_Msg)
            .WithErrorExample(404, ErrorCodes.ORD_003, ErrorCodes.ORD_003_Msg)
            .WithErrorExample(404, ErrorCodes.ORD_004, ErrorCodes.ORD_004_Msg)
            .WithErrorExample(422, ErrorCodes.ORD_005, ErrorCodes.ORD_005_Msg)
            .WithErrorExample(500, ErrorCodes.ORD_007, ErrorCodes.ORD_007_Msg);

            group.MapPut("/{id}/status", async (string id, UpdateOrderStatusRequest req, OrderRepository repo) =>
            {
                RequestValidator.Validate(req);

                var order = await GetOrderOrThrow(id, repo);

                if (!Transiciones[order.Estado].Contains(req.Estado))
                    throw new BusinessRuleException(
                        ErrorCodes.ORD_006,
                        $"Una orden en estado '{order.Estado}' no puede pasar a '{req.Estado}'.",
                        StatusCodes.Status409Conflict,
                        "No se puede modificar el estado.");

                var ahora = DateTime.UtcNow;
                await repo.UpdateStatusAsync(order.Id, req.Estado, ahora);

                return Results.Ok(new OrderStatusResponse(order.Id, req.Estado, ahora));
            })
            .WithSummary("Actualizar estado de la orden")
            .Produces<OrderStatusResponse>()
            .WithErrorExample(400, ErrorCodes.ORD_002, ErrorCodes.ORD_002_Msg)
            .WithErrorExample(404, ErrorCodes.ORD_001, ErrorCodes.ORD_001_Msg)
            .WithErrorExample(409, ErrorCodes.ORD_006, ErrorCodes.ORD_006_Msg)
            .WithErrorExample(500, ErrorCodes.ORD_007, ErrorCodes.ORD_007_Msg);
        }

        private static async Task<Order> GetOrderOrThrow(string id, OrderRepository repo)
        {
            var order = Guid.TryParse(id, out var guid) ? await repo.GetByIdAsync(guid) : null;
            return order ?? throw new NotFoundException(ErrorCodes.ORD_001, ErrorCodes.ORD_001_Msg);
        }
    }
}
