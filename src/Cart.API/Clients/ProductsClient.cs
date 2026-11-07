using System.Net;

namespace MiniApi.Clients;

public record ProductDto(Guid Id, string Nombre, decimal Precio, int Stock);

// Llama a Products.API para validar que el producto exista y tenga stock.
public class ProductsClient(HttpClient http)
{
    // Devuelve null si el producto no existe. Cualquier otro error de Products
    // lanza una excepción y termina en el 500 de este servicio.
    public async Task<ProductDto?> GetAsync(Guid id)
    {
        var response = await http.GetAsync($"/api/products/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }
}