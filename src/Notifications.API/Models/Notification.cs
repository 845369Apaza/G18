using System.ComponentModel.DataAnnotations;

namespace MiniApi.Models;

/// <summary>Notificación registrada.</summary>
public class Notification
{
    /// <summary>Identificador único.</summary>
    public Guid Id { get; set; }

    /// <summary>Usuario destinatario.</summary>
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
    public DateTime FechaEnvio { get; set; }
}

/// <summary>Datos para enviar una notificación.</summary>
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