using Dapper;
using Microsoft.Data.Sqlite;
using MiniApi.Models;

namespace MiniApi.Data
{
    public class OrderRepository
    {
        private const string OrderColumns =
            "id, usuario_id AS UsuarioId, total, estado, fecha_creacion AS FechaCreacion";

        private readonly IConfiguration _config;

        public OrderRepository(IConfiguration config) => _config = config;

        private SqliteConnection CreateConnection() =>
            new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=app.db");

        // ── GET ALL (filtro opcional por usuario) ─────────────────────────────
        public async Task<List<Order>> GetAllAsync(Guid? usuarioId)
        {
            using var conn = CreateConnection();

            var orders = (await conn.QueryAsync<Order>($"""
                SELECT {OrderColumns}
                FROM orders
                WHERE @usuarioId IS NULL OR usuario_id = @usuarioId
                ORDER BY fecha_creacion DESC
            """, new { usuarioId = usuarioId?.ToString() })).ToList();

            foreach (var order in orders)
                order.Items = await GetItemsAsync(conn, order.Id);

            return orders;
        }

        // ── GET BY ID ─────────────────────────────────────────────────────────
        public async Task<Order?> GetByIdAsync(Guid id)
        {
            using var conn = CreateConnection();

            var order = await conn.QuerySingleOrDefaultAsync<Order>(
                $"SELECT {OrderColumns} FROM orders WHERE id = @id", new { id });

            if (order is not null)
                order.Items = await GetItemsAsync(conn, order.Id);

            return order;
        }

        // ── CREATE: la orden y sus items se guardan juntos o no se guarda nada ─
        public async Task CreateAsync(Order order)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            await conn.ExecuteAsync("""
                INSERT INTO orders (id, usuario_id, total, estado, fecha_creacion)
                VALUES (@Id, @UsuarioId, @Total, @Estado, @FechaCreacion)
            """, order, tx);

            foreach (var item in order.Items)
            {
                await conn.ExecuteAsync("""
                    INSERT INTO order_items (order_id, producto_id, cantidad, precio_unitario)
                    VALUES (@OrderId, @ProductoId, @Cantidad, @PrecioUnitario)
                """, new { OrderId = order.Id, item.ProductoId, item.Cantidad, item.PrecioUnitario }, tx);
            }

            tx.Commit();
        }

        // ── UPDATE STATUS ─────────────────────────────────────────────────────
        public async Task UpdateStatusAsync(Guid id, string estado, DateTime fechaActualizacion)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("""
                UPDATE orders
                SET estado = @estado, fecha_actualizacion = @fechaActualizacion
                WHERE id = @id
            """, new { id, estado, fechaActualizacion });
        }

        private static async Task<List<OrderItem>> GetItemsAsync(SqliteConnection conn, Guid orderId)
        {
            var items = await conn.QueryAsync<OrderItem>("""
                SELECT producto_id AS ProductoId, cantidad, precio_unitario AS PrecioUnitario
                FROM order_items
                WHERE order_id = @orderId
            """, new { orderId });

            return items.ToList();
        }
    }
}
