using System.ComponentModel.DataAnnotations;

namespace MiniApi.Models;

// Entidad interna. Nunca se devuelve directo: tiene el PasswordHash.
public class User
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }
    public int IntentosFallidos { get; set; }
}

/// <summary>Datos para registrar un usuario.</summary>
/// <param name="Nombre" example="María">Nombre del usuario.</param>
/// <param name="Apellido" example="González">Apellido del usuario.</param>
/// <param name="Email" example="maria@email.com">Email único, con formato válido.</param>
/// <param name="Password" example="MiPassword123!">Contraseña. Se guarda hasheada.</param>
public record RegisterRequest(
    [property: Required(ErrorMessage = "El nombre es requerido")]
    string Nombre,

    [property: Required(ErrorMessage = "El apellido es requerido")]
    string Apellido,

    [property: Required(ErrorMessage = "El email es requerido")]
    [property: EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    string Email,

    [property: Required(ErrorMessage = "La contraseña es requerida")]
    string Password
);

/// <summary>Credenciales de acceso.</summary>
/// <param name="Email" example="maria@email.com">Email registrado.</param>
/// <param name="Password" example="MiPassword123!">Contraseña.</param>
public record LoginRequest(
    [property: Required(ErrorMessage = "El email es requerido")]
    [property: EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    string Email,

    [property: Required(ErrorMessage = "La contraseña es requerida")]
    string Password
);

/// <summary>Respuesta del registro.</summary>
/// <param name="Id" example="a1b2c3d4-0000-0000-0000-111122223333">Identificador único.</param>
/// <param name="Nombre" example="María">Nombre del usuario.</param>
/// <param name="Apellido" example="González">Apellido del usuario.</param>
/// <param name="Email" example="maria@email.com">Email del usuario.</param>
/// <param name="FechaRegistro" example="2024-03-10T09:00:00Z">Fecha de registro en UTC.</param>
/// <param name="Activo" example="true">false cuando el usuario está bloqueado.</param>
public record RegisterResponse(Guid Id, string Nombre, string Apellido, string Email, DateTime FechaRegistro, bool Activo);

/// <summary>Respuesta del login y de la consulta por id.</summary>
/// <param name="Id" example="a1b2c3d4-0000-0000-0000-111122223333">Identificador único.</param>
/// <param name="Nombre" example="María">Nombre del usuario.</param>
/// <param name="Apellido" example="González">Apellido del usuario.</param>
/// <param name="Email" example="maria@email.com">Email del usuario.</param>
public record UserResponse(Guid Id, string Nombre, string Apellido, string Email);
