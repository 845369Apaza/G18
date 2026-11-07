using Microsoft.AspNetCore.Identity;
using MiniApi.Data;
using MiniApi.Exceptions;
using MiniApi.Models;
using MiniApi.Swagger;

namespace MiniApi.Extensions.Endpoints
{
    public static class UsersEndpointsExtension
    {
        private const int MaxIntentos = 3;
        private static readonly PasswordHasher<User> Hasher = new();

        public static void MapUserEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/users").WithTags("Users");

            group.MapPost("/register", async (RegisterRequest req, UserRepository repo) =>
            {
                RequestValidator.Validate(req);

                if (await repo.GetByEmailAsync(req.Email) is not null)
                    throw new BusinessRuleException(
                        ErrorCodes.USR_001,
                        $"El email '{req.Email}' ya está registrado.",
                        StatusCodes.Status409Conflict,
                        "Ya existe un recurso con esos datos.");

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Nombre = req.Nombre,
                    Apellido = req.Apellido,
                    Email = req.Email,
                    FechaRegistro = DateTime.UtcNow,
                    Activo = true,
                    IntentosFallidos = 0
                };
                user.PasswordHash = Hasher.HashPassword(user, req.Password);

                await repo.CreateAsync(user);

                return Results.Created($"/api/users/{user.Id}", new RegisterResponse(
                    user.Id, user.Nombre, user.Apellido, user.Email, user.FechaRegistro, user.Activo));
            })
            .WithSummary("Registrar usuario")
            .Produces<RegisterResponse>(StatusCodes.Status201Created)
            .WithErrorExample(400, ErrorCodes.USR_002, ErrorCodes.USR_002_Msg)
            .WithErrorExample(409, ErrorCodes.USR_001, ErrorCodes.USR_001_Msg)
            .WithErrorExample(500, ErrorCodes.USR_006, ErrorCodes.USR_006_Msg);

            group.MapPost("/login", async (LoginRequest req, UserRepository repo) =>
            {
                RequestValidator.Validate(req);

                var user = await repo.GetByEmailAsync(req.Email) ?? throw BadCredentials();

                // Un usuario bloqueado no entra aunque la contraseña sea correcta.
                if (!user.Activo)
                    throw user.IntentosFallidos >= MaxIntentos
                        ? Forbidden(ErrorCodes.USR_004, ErrorCodes.USR_004_Msg)
                        : Forbidden(ErrorCodes.USR_005, ErrorCodes.USR_005_Msg);

                var result = Hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    var intentos = user.IntentosFallidos + 1;
                    await repo.UpdateLoginStateAsync(user.Id, intentos, activo: intentos < MaxIntentos);
                    throw BadCredentials();
                }

                await repo.UpdateLoginStateAsync(user.Id, intentosFallidos: 0, activo: true);

                return Results.Ok(new UserResponse(user.Id, user.Nombre, user.Apellido, user.Email));
            })
            .WithSummary("Iniciar sesión")
            .Produces<UserResponse>()
            .WithErrorExample(400, ErrorCodes.USR_002, ErrorCodes.USR_002_Msg)
            .WithErrorExample(401, ErrorCodes.USR_003, ErrorCodes.USR_003_Msg)
            .WithErrorExample(403, ErrorCodes.USR_004, ErrorCodes.USR_004_Msg)
            .WithErrorExample(403, ErrorCodes.USR_005, ErrorCodes.USR_005_Msg)
            .WithErrorExample(500, ErrorCodes.USR_006, ErrorCodes.USR_006_Msg);

            // No figura en la consigna: Orders y Notifications lo necesitan para validar al usuario.
            group.MapGet("/{id}", async (string id, UserRepository repo) =>
            {
                if (!Guid.TryParse(id, out var guid))
                    return Results.NotFound();

                var user = await repo.GetByIdAsync(guid);
                return user is null
                    ? Results.NotFound()
                    : Results.Ok(new UserResponse(user.Id, user.Nombre, user.Apellido, user.Email));
            })
            .WithSummary("Consultar usuario (uso interno entre servicios)")
            .Produces<UserResponse>()
            .Produces(StatusCodes.Status404NotFound);

            // Solo en desarrollo: permite demostrar USR-005 en la defensa.
            if (app.Environment.IsDevelopment())
            {
                group.MapPost("/{id:guid}/bloquear", async (Guid id, UserRepository repo) =>
                {
                    await repo.UpdateLoginStateAsync(id, intentosFallidos: 0, activo: false);
                    return Results.NoContent();
                })
                .WithSummary("Bloqueo manual por fraude (solo desarrollo)");
            }
        }

        private static BusinessRuleException BadCredentials() =>
            new(ErrorCodes.USR_003, ErrorCodes.USR_003_Msg,
                StatusCodes.Status401Unauthorized, "Las credenciales no son válidas.");

        private static BusinessRuleException Forbidden(string code, string message) =>
            new(code, message, StatusCodes.Status403Forbidden, "El acceso está prohibido.");
    }
}