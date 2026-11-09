using Dapper;
using Microsoft.Data.Sqlite;
using MiniApi.Models;

namespace MiniApi.Data
{
    public class NotificationRepository
    {
        private readonly IConfiguration _config;

        public NotificationRepository(IConfiguration config) => _config = config;

        private SqliteConnection CreateConnection() =>
            new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=app.db");

        public async Task CreateAsync(Notification notification)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("""
                INSERT INTO notifications (id, usuario_id, mensaje, tipo, estado, fecha_envio)
                VALUES (@Id, @UsuarioId, @Mensaje, @Tipo, @Estado, @FechaEnvio)
            """, notification);
        }

        public async Task<List<Notification>> GetByUserAsync(Guid usuarioId)
        {
            using var conn = CreateConnection();
            var rows = await conn.QueryAsync<Notification>("""
                SELECT id, usuario_id AS UsuarioId, mensaje, tipo, estado, fecha_envio AS FechaEnvio
                FROM notifications
                WHERE usuario_id = @usuarioId
                ORDER BY fecha_envio DESC
            """, new { usuarioId });

            return rows.ToList();
        }
    }
}
