using System.Text.Json;
using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Infrastructure.Integrations;

public class CnpjApiClient : ICnpjApiClient
{
    private readonly HttpClient _httpClient;

    public CnpjApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CnpjResponseDTO?> ConsultarCnpj(string cnpj)
    {
        try
        {
            return await ConsultarBrasilApi(cnpj);
        }
        catch (Exception exBrasilApi)
        {
            try
            {
                return await ConsultarReceitaWs(cnpj);
            }
            catch (Exception exReceitaWs)
            {
                throw new Exception($"Erro nas APIs de CNPJ: BrasilAPI -> {exBrasilApi.Message} | ReceitaWS -> {exReceitaWs.Message}");
            }
        }
    }

    //  BRASIL API
    private async Task<CnpjResponseDTO?> ConsultarBrasilApi(string cnpj)
    {
        var response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/cnpj/v1/{cnpj}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"BrasilAPI erro: {response.StatusCode} - {error}");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (json.ValueKind == JsonValueKind.Undefined)
            throw new Exception("BrasilAPI retornou vazio");

        return new CnpjResponseDTO
        {
            Cnpj = json.GetProperty("cnpj").GetString() ?? "",
            RazaoSocial = json.GetProperty("razao_social").GetString() ?? "",
            NomeFantasia = json.TryGetProperty("nome_fantasia", out var fantasia) ? fantasia.GetString() ?? "" : "",
            SituacaoCadastral = json.GetProperty("descricao_situacao_cadastral").GetString() ?? "",
            Logradouro = json.GetProperty("logradouro").GetString() ?? "",
            Municipio = json.GetProperty("municipio").GetString() ?? "",
            Uf = json.GetProperty("uf").GetString() ?? ""
        };
    }

    // RECEITA WS (fallback)
    private async Task<CnpjResponseDTO?> ConsultarReceitaWs(string cnpj)
    {
        var response = await _httpClient.GetAsync($"https://www.receitaws.com.br/v1/cnpj/{cnpj}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"ReceitaWS erro: {response.StatusCode} - {error}");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (json.ValueKind == JsonValueKind.Undefined)
            throw new Exception("ReceitaWS retornou vazio");

        // ReceitaWS retorna "status":"ERROR" quando não acha
        if (json.TryGetProperty("status", out var status) && status.GetString() == "ERROR")
        {
            var message = json.TryGetProperty("message", out var msg) ? msg.GetString() : "Erro desconhecido";
            throw new Exception($"ReceitaWS: {message}");
        }

        return new CnpjResponseDTO
        {
            Cnpj = json.GetProperty("cnpj").GetString() ?? "",
            RazaoSocial = json.GetProperty("nome").GetString() ?? "",
            NomeFantasia = json.TryGetProperty("fantasia", out var fantasia) ? fantasia.GetString() ?? "" : "",
            SituacaoCadastral = json.GetProperty("situacao").GetString() ?? "",
            Logradouro = json.GetProperty("logradouro").GetString() ?? "",
            Municipio = json.GetProperty("municipio").GetString() ?? "",
            Uf = json.GetProperty("uf").GetString() ?? ""
        };
    }
}