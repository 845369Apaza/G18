using System.ComponentModel.DataAnnotations;

namespace MiniApi.Models;

/// <summary>Orden de compra.</summary>
public class Order
{
    /// <summary>Identificador único.</summary>
    /// <example>f1e2d3c4-0000-0000-0000-aabbccddeeff</example>
    public Guid Id { get; set; }

    /// <summary>Usuario que realizó la orden.</summary>
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid UsuarioId { get; set; }

    /// <summary>Productos incluidos.</summary>
    public List<OrderItem> Items { get; set; } = [];

    /// <summary>Total calculado al crear la orden.</summary>
    /// <example>3000.00</example>
    public decimal Total { get; set; }

    /// <summary>Pendiente, Confirmada, Enviada, Entregada o Cancelada.</summary>
    /// <example>Pendiente</example>
    public string Estado { get; set; } = string.Empty;

    /// <summary>Fecha de creación en UTC.</summary>
    /// <example>2024-03-10T11:00:00Z</example>
    public DateTime FechaCreacion { get; set; }
}

/// <summary>Producto dentro de una orden.</summary>
public class OrderItem
{
    /// <summary>Referencia al producto.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductoId { get; set; }

    /// <summary>Unidades.</summary>
    /// <example>2</example>
    public int Cantidad { get; set; }

    /// <summary>Precio capturado al crear la orden.</summary>
    /// <example>1500.00</example>
    public decimal PrecioUnitario { get; set; }
}

/// <summary>Datos para crear una orden.</summary>
/// <param name="UsuarioId" example="a1b2c3d4-0000-0000-0000-111122223333">Usuario que compra. Tiene que existir en Users.</param>
/// <param name="Items">Productos de la orden. Al menos uno.</param>
public record CreateOrderRequest(
    [property: Required(ErrorMessage = "El usuarioId es requerido")]
    Guid? UsuarioId,

    [property: Required(ErrorMessage = "La lista de items es requerida")]
    [property: MinLength(1, ErrorMessage = "La orden debe tener al menos un item")]
    List<CreateOrderItemRequest> Items
);

/// <summary>Producto a incluir en la orden.</summary>
/// <param name="ProductoId" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">Producto. Tiene que existir en Products.</param>
/// <param name="Cantidad" example="2">Unidades. Mayor a 0.</param>
public record CreateOrderItemRequest(
    [property: Required(ErrorMessage = "El productoId es requerido")]
    Guid? ProductoId,

    [property: Required(ErrorMessage = "La cantidad es requerida")]
    [property: Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    int? Cantidad
);

/// <summary>Nuevo estado de la orden.</summary>
/// <param name="Estado" example="Confirmada">Pendiente, Confirmada, Enviada, Entregada o Cancelada.</param>
public record UpdateOrderStatusRequest(
    [property: Required(ErrorMessage = "El estado es requerido")]
    [property: RegularExpression("^(Pendiente|Confirmada|Enviada|Entregada|Cancelada)$",
        ErrorMessage = "El estado no es válido")]
    string Estado
);

/// <summary>Respuesta al actualizar el estado.</summary>
/// <param name="Id" example="f1e2d3c4-0000-0000-0000-aabbccddeeff">Identificador de la orden.</param>
/// <param name="Estado" example="Confirmada">Estado nuevo.</param>
/// <param name="FechaActualizacion" example="2024-03-10T12:00:00Z">Fecha del cambio en UTC.</param>
public record OrderStatusResponse(Guid Id, string Estado, DateTime FechaActualizacion);
