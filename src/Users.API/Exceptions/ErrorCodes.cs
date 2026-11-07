namespace MiniApi.Exceptions;

/// <summary>Catálogo de errores de Users API (sección 4.2 de la consigna).</summary>
public static class ErrorCodes
{
    public const string USR_001 = "USR-001";
    public const string USR_001_Msg = "El email ya está registrado.";
    public const string USR_002 = "USR-002";
    public const string USR_002_Msg = "Los datos del usuario son inválidos.";
    public const string USR_003 = "USR-003";
    public const string USR_003_Msg = "Credenciales incorrectas.";
    public const string USR_004 = "USR-004";
    public const string USR_004_Msg = "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte.";
    public const string USR_005 = "USR-005";
    public const string USR_005_Msg = "Su cuenta fue suspendida por razones de seguridad. Contacte a soporte.";
    public const string USR_006 = "USR-006";
    public const string USR_006_Msg = "Error interno al procesar el usuario.";

    // Lo que los handlers genéricos necesitan saber de este servicio.
    public const string InvalidData = USR_002;
    public const string InvalidDataMsg = USR_002_Msg;
    public const string Internal = USR_006;
    public const string InternalMsg = USR_006_Msg;
}