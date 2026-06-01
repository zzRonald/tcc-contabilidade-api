using System.Text.Json;
using System.Net.Http.Json;
using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Infrastructure.Integrations;

public class CnpjIntegrationService : ExternalApiClient, ICnpjApiClient, IExternalIntegration
{
    public string NomeIntegracao => "API de Consulta de CNPJ (BrasilAPI/ReceitaWS)";
    public bool Ativo => true;

    public CnpjIntegrationService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<bool> TestarConexaoAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync("https://brasilapi.com.br/api/cnpj/v1/00000000000191");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
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

    private async Task<CnpjResponseDTO?> ConsultarBrasilApi(string cnpj)
    {
        var json = await GetAsync<JsonElement>($"https://brasilapi.com.br/api/cnpj/v1/{cnpj}");

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

    private async Task<CnpjResponseDTO?> ConsultarReceitaWs(string cnpj)
    {
        var json = await GetAsync<JsonElement>($"https://www.receitaws.com.br/v1/cnpj/{cnpj}");

        if (json.ValueKind == JsonValueKind.Undefined)
            throw new Exception("ReceitaWS retornou vazio");

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
