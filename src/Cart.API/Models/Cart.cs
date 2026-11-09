using System.ComponentModel.DataAnnotations;

namespace MiniApi.Models;

/// <summary>Carrito de un usuario.</summary>
public class Cart
{
    /// <summary>Usuario dueño del carrito.</summary>
    /// <example>a1b2c3d4-0000-0000-0000-111122223333</example>
    public Guid UsuarioId { get; set; }

    /// <summary>Productos en el carrito.</summary>
    public List<CartItem> Items { get; set; } = [];

    /// <summary>Última modificación en UTC.</summary>
    /// <example>2024-03-10T10:45:00Z</example>
    public DateTime FechaActualizacion { get; set; }
}

/// <summary>Producto dentro del carrito.</summary>
public class CartItem
{
    /// <summary>Referencia al producto.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductoId { get; set; }

    /// <summary>Unidades.</summary>
    /// <example>2</example>
    public int Cantidad { get; set; }
}

/// <summary>Producto a agregar.</summary>
/// <param name="ProductoId" example="3fa85f64-5717-4562-b3fc-2c963f66afa6">Producto a agregar.</param>
/// <param name="Cantidad" example="2">Unidades a sumar al carrito. Mayor a cero.</param>
public record AddCartItemRequest(
    [property: Required(ErrorMessage = "El productoId es requerido")]
    Guid? ProductoId,

    [property: Required(ErrorMessage = "La cantidad es requerida")]
    [property: Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    int? Cantidad
);

/// <summary>Nueva cantidad de un item.</summary>
/// <param name="Cantidad" example="4">Cantidad final del item. Mayor a cero.</param>
public record UpdateCartItemRequest(
    [property: Required(ErrorMessage = "La cantidad es requerida")]
    [property: Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    int? Cantidad
);
