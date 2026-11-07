namespace MiniApi.Exceptions;

/// <summary>Catálogo de errores de Cart API (sección 4.4 de la consigna).</summary>
public static class ErrorCodes
{
    public const string CRT_001 = "CRT-001";
    public const string CRT_001_Msg = "Carrito no encontrado.";
    public const string CRT_002 = "CRT-002";
    public const string CRT_002_Msg = "Producto no encontrado.";
    public const string CRT_003 = "CRT-003";
    public const string CRT_003_Msg = "Stock insuficiente para agregar al carrito.";
    public const string CRT_004 = "CRT-004";
    public const string CRT_004_Msg = "Cantidad inválida.";
    public const string CRT_005 = "CRT-005";
    public const string CRT_005_Msg = "Error interno al procesar el carrito.";

    // Lo que los handlers genéricos necesitan saber de este servicio.
    public const string InvalidData = CRT_004;
    public const string InvalidDataMsg = CRT_004_Msg;
    public const string Internal = CRT_005;
    public const string InternalMsg = CRT_005_Msg;
}
