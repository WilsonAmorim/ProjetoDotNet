
using PpeDashboard.Client.DTOs;
using System.Net.Http.Json;

namespace PpeDashboard.Client.Services;

public class ConveniosClientService
{
    private readonly HttpClient _httpClient;

    public ConveniosClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ConveniosDTO>> GetConveniosAsync()
    {
        try
        {
            // Endpoint que corresponde ao seu ConveniosController
            var convenios = await _httpClient.GetFromJsonAsync<List<ConveniosDTO>>("api/Convenios");
            return convenios ?? new List<ConveniosDTO>();
        }
        catch (Exception ex)
        {
            // Aqui você pode logar o erro
            Console.WriteLine($"Erro na requisição de Convênios: {ex.Message}");
            return new List<ConveniosDTO>();
        }
    }
}