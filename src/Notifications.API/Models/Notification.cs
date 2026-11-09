using System.ComponentModel.DataAnnotations;

namespace MiniApi.Models;

/// <summary>Notificación registrada.</summary>
public class Notification
{
    /// <summary>Identificador único.</summary>
    /// <example>11112222-3333-4444-5555-666677778888</example>
    public Guid Id { get; set; }

    /// <summary>Usuario destinatario.</summary>
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid UsuarioId { get; set; }

    /// <summary>Texto del mensaje.</summary>
    /// <example>Su orden fue confirmada.</example>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>Email, Push o SMS.</summary>
    /// <example>Email</example>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>Pendiente, Enviada o Fallida.</summary>
    /// <example>Enviada</example>
    public string Estado { get; set; } = string.Empty;

    /// <summary>Fecha de envío en UTC.</summary>
    /// <example>2024-03-10T12:01:00Z</example>
    public DateTime FechaEnvio { get; set; }
}

/// <summary>Datos para enviar una notificación.</summary>
/// <param name="UsuarioId" example="a1b2c3d4-0000-0000-0000-111122223333">Usuario destinatario. Tiene que existir en Users.</param>
/// <param name="Mensaje" example="Su orden fue confirmada.">Texto del mensaje. Hasta 500 caracteres.</param>
/// <param name="Tipo" example="Email">Email, Push o SMS.</param>
public record SendNotificationRequest(
    [property: Required(ErrorMessage = "El usuarioId es requerido")]
    Guid? UsuarioId,

    [property: Required(ErrorMessage = "El mensaje es requerido")]
    [property: StringLength(500, ErrorMessage = "El mensaje admite hasta 500 caracteres")]
    string Mensaje,

    [property: Required(ErrorMessage = "El tipo es requerido")]
    [property: RegularExpression("^(Email|Push|SMS)$", ErrorMessage = "El tipo debe ser Email, Push o SMS")]
    string Tipo
);
