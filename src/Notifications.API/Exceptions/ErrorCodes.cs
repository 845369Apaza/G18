namespace MiniApi.Exceptions;

/// <summary>Catálogo de errores de Notifications API (sección 4.5 de la consigna).</summary>
public static class ErrorCodes
{
    public const string NTF_001 = "NTF-001";
    public const string NTF_001_Msg = "El usuario destinatario no fue encontrado.";
    public const string NTF_002 = "NTF-002";
    public const string NTF_002_Msg = "Los datos de la notificación son inválidos.";
    public const string NTF_003 = "NTF-003";
    public const string NTF_003_Msg = "No se encontraron notificaciones para el usuario.";
    public const string NTF_004 = "NTF-004";
    public const string NTF_004_Msg = "Error interno al procesar la notificación.";

    // Lo que los handlers genéricos necesitan saber de este servicio.
    public const string InvalidData = NTF_002;
    public const string InvalidDataMsg = NTF_002_Msg;
    public const string Internal = NTF_004;
    public const string InternalMsg = NTF_004_Msg;
}