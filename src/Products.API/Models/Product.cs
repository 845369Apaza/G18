using System.ComponentModel.DataAnnotations;

namespace MiniApi.Models;

/// <summary>Producto del catálogo.</summary>
public class Product
{
    /// <summary>Identificador único.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>Nombre comercial.</summary>
    /// <example>Notebook Dell XPS 15</example>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción opcional.</summary>
    /// <example>Laptop 15 pulgadas, 32GB RAM</example>
    public string? Descripcion { get; set; }

    /// <summary>Precio unitario.</summary>
    /// <example>1500.00</example>
    public decimal Precio { get; set; }

    /// <summary>Unidades disponibles.</summary>
    /// <example>10</example>
    public int Stock { get; set; }

    /// <summary>Categoría informativa, no se valida contra una lista.</summary>
    /// <example>Electrónica</example>
    public string Categoria { get; set; } = string.Empty;

    /// <summary>Fecha de alta en UTC.</summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime FechaCreacion { get; set; }
}

/// <summary>Datos para crear o actualizar un producto.</summary>
public record ProductRequest(
    [property: Required(ErrorMessage = "El nombre es requerido")]
    [property: StringLength(100, ErrorMessage = "El nombre admite hasta 100 caracteres")]
    string Nombre,

    [property: StringLength(500, ErrorMessage = "La descripción admite hasta 500 caracteres")]
    string? Descripcion,

    [property: Required(ErrorMessage = "El precio es requerido")]
    [property: Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    decimal? Precio,

    [property: Required(ErrorMessage = "El stock es requerido")]
    [property: Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
    int? Stock,

    [property: Required(ErrorMessage = "La categoría es requerida")]
    string Categoria
);