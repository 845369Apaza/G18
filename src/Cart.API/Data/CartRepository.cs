using Dapper;
using Microsoft.Data.Sqlite;
using MiniApi.Models;

namespace MiniApi.Data
{
    public class CartRepository
    {
        private readonly IConfiguration _config;

        public CartRepository(IConfiguration config) => _config = config;

        private SqliteConnection CreateConnection() =>
            new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=app.db");

        // ── Leer el carrito con sus items ─────────────────────────────────────
        public async Task<Cart?> GetAsync(Guid usuarioId)
        {
            using var conn = CreateConnection();

            var cart = await conn.QuerySingleOrDefaultAsync<Cart>("""
                SELECT usuario_id AS UsuarioId, fecha_actualizacion AS FechaActualizacion
                FROM carts
                WHERE usuario_id = @usuarioId
            """, new { usuarioId });

            if (cart is null) return null;

            var items = await conn.QueryAsync<CartItem>("""
                SELECT producto_id AS ProductoId, cantidad
                FROM cart_items
                WHERE usuario_id = @usuarioId
            """, new { usuarioId });

            cart.Items = items.ToList();
            return cart;
        }

        // ── Agregar o reemplazar la cantidad de un item ───────────────────────
        // Crea el carrito si todavía no existe.
        public async Task SaveItemAsync(Guid usuarioId, Guid productoId, int cantidad)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            await conn.ExecuteAsync("""
                INSERT INTO carts (usuario_id, fecha_actualizacion)
                VALUES (@usuarioId, @ahora)
                ON CONFLICT(usuario_id) DO UPDATE SET fecha_actualizacion = @ahora
            """, new { usuarioId, ahora = DateTime.UtcNow }, tx);

            await conn.ExecuteAsync("""
                INSERT INTO cart_items (usuario_id, producto_id, cantidad)
                VALUES (@usuarioId, @productoId, @cantidad)
                ON CONFLICT(usuario_id, producto_id) DO UPDATE SET cantidad = @cantidad
            """, new { usuarioId, productoId, cantidad }, tx);

            tx.Commit();
        }

        // ── Quitar un item ────────────────────────────────────────────────────
        public async Task<bool> RemoveItemAsync(Guid usuarioId, Guid productoId)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            var rows = await conn.ExecuteAsync("""
                DELETE FROM cart_items
                WHERE usuario_id = @usuarioId AND producto_id = @productoId
            """, new { usuarioId, productoId }, tx);

            await conn.ExecuteAsync("""
                UPDATE carts SET fecha_actualizacion = @ahora WHERE usuario_id = @usuarioId
            """, new { usuarioId, ahora = DateTime.UtcNow }, tx);

            tx.Commit();
            return rows > 0;
        }

        // ── Vaciar el carrito completo ────────────────────────────────────────
        public async Task<bool> DeleteAsync(Guid usuarioId)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            await conn.ExecuteAsync("DELETE FROM cart_items WHERE usuario_id = @usuarioId", new { usuarioId }, tx);
            var rows = await conn.ExecuteAsync("DELETE FROM carts WHERE usuario_id = @usuarioId", new { usuarioId }, tx);

            tx.Commit();
            return rows > 0;
        }
    }
}