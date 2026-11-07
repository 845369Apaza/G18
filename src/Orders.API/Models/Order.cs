using System.ComponentModel.DataAnnotations;

namespace MiniApi.Models;

/// <summary>Orden de compra.</summary>
public class Order
{
    /// <summary>Identificador único.</summary>
    public Guid Id { get; set; }

    /// <summary>Usuario que realizó la orden.</summary>
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
    public DateTime FechaCreacion { get; set; }
}

/// <summary>Producto dentro de una orden.</summary>
public class OrderItem
{
    /// <summary>Referencia al producto.</summary>
    public Guid ProductoId { get; set; }

    /// <summary>Unidades.</summary>
    /// <example>2</example>
    public int Cantidad { get; set; }

    /// <summary>Precio capturado al crear la orden.</summary>
    /// <example>1500.00</example>
    public decimal PrecioUnitario { get; set; }
}

/// <summary>Datos para crear una orden.</summary>
public record CreateOrderRequest(
    [property: Required(ErrorMessage = "El usuarioId es requerido")]
    Guid? UsuarioId,

    [property: Required(ErrorMessage = "La lista de items es requerida")]
    [property: MinLength(1, ErrorMessage = "La orden debe tener al menos un item")]
    List<CreateOrderItemRequest> Items
);

/// <summary>Producto a incluir en la orden.</summary>
public record CreateOrderItemRequest(
    [property: Required(ErrorMessage = "El productoId es requerido")]
    Guid? ProductoId,

    [property: Required(ErrorMessage = "La cantidad es requerida")]
    [property: Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    int? Cantidad
);

/// <summary>Nuevo estado de la orden.</summary>
public record UpdateOrderStatusRequest(
    [property: Required(ErrorMessage = "El estado es requerido")]
    [property: RegularExpression("^(Pendiente|Confirmada|Enviada|Entregada|Cancelada)$",
        ErrorMessage = "El estado no es válido")]
    string Estado
);

/// <summary>Respuesta al actualizar el estado.</summary>
public record OrderStatusResponse(Guid Id, string Estado, DateTime FechaActualizacion);