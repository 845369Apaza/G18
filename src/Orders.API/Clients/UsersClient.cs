using System.Net;

namespace MiniApi.Clients;

// Llama a Users.API para validar que el usuario exista.
public class UsersClient(HttpClient http)
{
    public async Task<bool> ExistsAsync(Guid id)
    {
        var response = await http.GetAsync($"/api/users/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        response.EnsureSuccessStatusCode();
        return true;
    }
}
