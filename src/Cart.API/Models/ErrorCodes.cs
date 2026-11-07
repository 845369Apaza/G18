namespace MiniApi.Models;

/// <summary>Catálogo de errores de este servicio (sección 4.1 de la consigna).</summary>
public static class ErrorCodes
{
    public const string PRD_001 = "PRD-001";
    public const string PRD_001_Msg = "Producto no encontrado.";
    public const string PRD_002 = "PRD-002";
    public const string PRD_002_Msg = "Los datos del producto son inválidos.";
    public const string PRD_003 = "PRD-003";
    public const string PRD_003_Msg = "Ya existe un producto con ese nombre en la categoría.";
    public const string PRD_004 = "PRD-004";
    public const string PRD_004_Msg = "El producto tiene órdenes activas y no puede eliminarse.";
    public const string PRD_005 = "PRD-005";
    public const string PRD_005_Msg = "Error interno al procesar el producto.";

    // Lo que los handlers genéricos necesitan saber de este servicio.
    public const string InvalidData = PRD_002;
    public const string InvalidDataMsg = PRD_002_Msg;
    public const string Internal = PRD_005;
    public const string InternalMsg = PRD_005_Msg;
}
