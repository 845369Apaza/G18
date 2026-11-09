using System.ComponentModel.DataAnnotations;

namespace MiniApi.Models;

/// <summary>Carrito de un usuario.</summary>
public class Cart
{
    /// <summary>Usuario dueño del carrito.</summary>
    public Guid UsuarioId { get; set; }

    /// <summary>Productos en el carrito.</summary>
    public List<CartItem> Items { get; set; } = [];

    /// <summary>Última modificación en UTC.</summary>
    public DateTime FechaActualizacion { get; set; }
}

/// <summary>Producto dentro del carrito.</summary>
public class CartItem
{
    /// <summary>Referencia al producto.</summary>
    public Guid ProductoId { get; set; }

    /// <summary>Unidades.</summary>
    /// <example>2</example>
    public int Cantidad { get; set; }
}

/// <summary>Producto a agregar.</summary>
public record AddCartItemRequest(
    [property: Required(ErrorMessage = "El productoId es requerido")]
    Guid? ProductoId,

    [property: Required(ErrorMessage = "La cantidad es requerida")]
    [property: Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    int? Cantidad
);

/// <summary>Nueva cantidad de un item.</summary>
public record UpdateCartItemRequest(
    [property: Required(ErrorMessage = "La cantidad es requerida")]
    [property: Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    int? Cantidad
);
