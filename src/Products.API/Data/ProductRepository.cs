using Dapper;
using Microsoft.Data.Sqlite;
using MiniApi.Models;

namespace MiniApi.Data
{
    public class ProductRepository
    {
        private const string Columns =
            "id, nombre, descripcion, precio, stock, categoria, fecha_creacion AS FechaCreacion";

        private readonly IConfiguration _config;

        public ProductRepository(IConfiguration config) => _config = config;

        private SqliteConnection CreateConnection() =>
            new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=app.db");

        // ── GET ALL (con filtros opcionales) ──────────────────────────────────
        public async Task<IEnumerable<Product>> GetAllAsync(string? categoria, string? nombre)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<Product>($"""
                SELECT {Columns}
                FROM products
                WHERE (@categoria IS NULL OR categoria = @categoria COLLATE NOCASE)
                  AND (@nombre IS NULL OR nombre LIKE '%' || @nombre || '%')
                ORDER BY fecha_creacion DESC
            """, new { categoria, nombre });
        }

        // ── GET BY ID ─────────────────────────────────────────────────────────
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            using var conn = CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<Product>($"""
                SELECT {Columns}
                FROM products
                WHERE id = @id
            """, new { id });
        }

        // ── ¿Existe ese nombre en esa categoría? (PRD-003) ────────────────────
        public async Task<bool> ExistsAsync(string nombre, string categoria)
        {
            using var conn = CreateConnection();
            return await conn.ExecuteScalarAsync<bool>("""
                SELECT EXISTS(
                    SELECT 1 FROM products
                    WHERE nombre = @nombre COLLATE NOCASE
                      AND categoria = @categoria COLLATE NOCASE)
            """, new { nombre, categoria });
        }

        // ── CREATE ────────────────────────────────────────────────────────────
        public async Task<Product> CreateAsync(ProductRequest request)
        {
            using var conn = CreateConnection();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio!.Value,
                Stock = request.Stock!.Value,
                Categoria = request.Categoria,
                FechaCreacion = DateTime.UtcNow
            };

            await conn.ExecuteAsync("""
                INSERT INTO products (id, nombre, descripcion, precio, stock, categoria, fecha_creacion)
                VALUES (@Id, @Nombre, @Descripcion, @Precio, @Stock, @Categoria, @FechaCreacion)
            """, product);

            return product;
        }

        // ── UPDATE ────────────────────────────────────────────────────────────
        public async Task<bool> UpdateAsync(Guid id, ProductRequest request)
        {
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync("""
                UPDATE products
                SET nombre      = @Nombre,
                    descripcion = @Descripcion,
                    precio      = @Precio,
                    stock       = @Stock,
                    categoria   = @Categoria
                WHERE id = @Id
            """, new { request.Nombre, request.Descripcion, request.Precio, request.Stock, request.Categoria, Id = id });

            return rows > 0;
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        public async Task<bool> DeleteAsync(Guid id)
        {
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync("DELETE FROM products WHERE id = @id", new { id });
            return rows > 0;
        }
    }
}