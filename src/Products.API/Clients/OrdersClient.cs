namespace MiniApi.Clients;

// Llama a Orders.API para saber si un producto se puede eliminar (PRD-004).
public class OrdersClient(HttpClient http)
{
    private record OrderDto(string Estado, List<OrderItemDto> Items);
    private record OrderItemDto(Guid ProductoId);

    public async Task<bool> HasActiveOrdersAsync(Guid productId)
    {
        var response = await http.GetAsync("/api/orders");
        response.EnsureSuccessStatusCode();

        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>() ?? [];

        return orders.Any(order =>
            order.Estado is "Pendiente" or "Confirmada" &&
            order.Items.Any(item => item.ProductoId == productId));
    }
}
