using MiniApi.Clients;
using MiniApi.Data;
using MiniApi.Exceptions;
using MiniApi.Models;
using MiniApi.Swagger;

namespace MiniApi.Extensions.Endpoints
{
    public static class NotificationsEndpointsExtension
    {
        public static void MapNotificationEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/notifications").WithTags("Notifications");

            group.MapPost("/send", async (SendNotificationRequest req, NotificationRepository repo, UsersClient users) =>
            {
                RequestValidator.Validate(req);

                if (!await users.ExistsAsync(req.UsuarioId!.Value))
                    throw new NotFoundException(ErrorCodes.NTF_001, ErrorCodes.NTF_001_Msg);

                // El envío es simulado: se registra directamente como Enviada.
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = req.UsuarioId.Value,
                    Mensaje = req.Mensaje,
                    Tipo = req.Tipo,
                    Estado = "Enviada",
                    FechaEnvio = DateTime.UtcNow
                };

                await repo.CreateAsync(notification);
                return Results.Created($"/api/notifications/{notification.UsuarioId}", notification);
            })
            .WithSummary("Registrar y simular el envío de una notificación")
            .Produces<Notification>(StatusCodes.Status201Created)
            .WithErrorExample(400, ErrorCodes.NTF_002, ErrorCodes.NTF_002_Msg)
            .WithErrorExample(404, ErrorCodes.NTF_001, ErrorCodes.NTF_001_Msg)
            .WithErrorExample(500, ErrorCodes.NTF_004, ErrorCodes.NTF_004_Msg);

            group.MapGet("/{userId}", async (string userId, NotificationRepository repo) =>
            {
                var notifications = Guid.TryParse(userId, out var guid)
                    ? await repo.GetByUserAsync(guid)
                    : [];

                if (notifications.Count == 0)
                    throw new NotFoundException(ErrorCodes.NTF_003, ErrorCodes.NTF_003_Msg);

                return Results.Ok(notifications);
            })
            .WithSummary("Listar notificaciones de un usuario")
            .Produces<List<Notification>>()
            .WithErrorExample(404, ErrorCodes.NTF_003, ErrorCodes.NTF_003_Msg)
            .WithErrorExample(500, ErrorCodes.NTF_004, ErrorCodes.NTF_004_Msg);
        }
    }
}