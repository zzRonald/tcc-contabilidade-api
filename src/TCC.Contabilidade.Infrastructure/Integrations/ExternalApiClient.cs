using System.Net.Http.Json;
using System.Text.Json;

namespace TCC.Contabilidade.Infrastructure.Integrations;

public abstract class ExternalApiClient
{
    protected readonly HttpClient HttpClient;

    protected ExternalApiClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    protected async Task<T?> GetAsync<T>(string url)
    {
        var response = await HttpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Erro na requisição externa: {response.StatusCode} - {error}");
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var response = await HttpClient.PostAsJsonAsync(url, data);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Erro na requisição externa (POST): {response.StatusCode} - {error}");
    }
}
