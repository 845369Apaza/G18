namespace MiniApi.Exceptions;

/// <summary>Catálogo de errores de Orders API (sección 4.3 de la consigna).</summary>
public static class ErrorCodes
{
    public const string ORD_001 = "ORD-001";
    public const string ORD_001_Msg = "Orden no encontrada.";
    public const string ORD_002 = "ORD-002";
    public const string ORD_002_Msg = "Los datos de la orden son inválidos.";
    public const string ORD_003 = "ORD-003";
    public const string ORD_003_Msg = "Usuario no encontrado al crear la orden.";
    public const string ORD_004 = "ORD-004";
    public const string ORD_004_Msg = "Producto no encontrado al crear la orden.";
    public const string ORD_005 = "ORD-005";
    public const string ORD_005_Msg = "Stock insuficiente para uno o más productos.";
    public const string ORD_006 = "ORD-006";
    public const string ORD_006_Msg = "El estado de la orden no puede ser modificado.";
    public const string ORD_007 = "ORD-007";
    public const string ORD_007_Msg = "Error interno al procesar la orden.";

    // Lo que los handlers genéricos necesitan saber de este servicio.
    public const string InvalidData = ORD_002;
    public const string InvalidDataMsg = ORD_002_Msg;
    public const string Internal = ORD_007;
    public const string InternalMsg = ORD_007_Msg;
}