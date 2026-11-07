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
public record LoginRequest(
    [property: Required(ErrorMessage = "El email es requerido")]
    [property: EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    string Email,

    [property: Required(ErrorMessage = "La contraseña es requerida")]
    string Password
);

/// <summary>Respuesta del registro.</summary>
public record RegisterResponse(Guid Id, string Nombre, string Apellido, string Email, DateTime FechaRegistro, bool Activo);

/// <summary>Respuesta del login y de la consulta por id.</summary>
public record UserResponse(Guid Id, string Nombre, string Apellido, string Email);
src/Users.API/Except