using Dapper;
using Microsoft.Data.Sqlite;
using MiniApi.Models;

namespace MiniApi.Data
{
    public class UserRepository
    {
        private const string Columns = """
            id, nombre, apellido, email, password_hash AS PasswordHash,
            fecha_registro AS FechaRegistro, activo, intentos_fallidos AS IntentosFallidos
            """;

        private readonly IConfiguration _config;

        public UserRepository(IConfiguration config) => _config = config;

        private SqliteConnection CreateConnection() =>
            new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=app.db");

        public async Task<User?> GetByIdAsync(Guid id)
        {
            using var conn = CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<User>(
                $"SELECT {Columns} FROM users WHERE id = @id", new { id });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var conn = CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<User>(
                $"SELECT {Columns} FROM users WHERE email = @email COLLATE NOCASE", new { email });
        }

        public async Task CreateAsync(User user)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("""
                INSERT INTO users (id, nombre, apellido, email, password_hash, fecha_registro, activo, intentos_fallidos)
                VALUES (@Id, @Nombre, @Apellido, @Email, @PasswordHash, @FechaRegistro, @Activo, @IntentosFallidos)
            """, user);
        }

        // Guarda intentos y estado juntos: el tercer fallo debe bloquear en la misma operación.
        public async Task UpdateLoginStateAsync(Guid id, int intentosFallidos, bool activo)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("""
                UPDATE users
                SET intentos_fallidos = @intentosFallidos, activo = @activo
                WHERE id = @id
            """, new { id, intentosFallidos, activo });
        }
    }
}
