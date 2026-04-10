using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Application.Services;

public class CnpjService
{
    private readonly ICnpjApiClient _client;

    public CnpjService(ICnpjApiClient client)
    {
        _client = client;
    }

    public async Task<CnpjResponseDTO> ConsultarCnpj(string cnpj, TipoUsuario tipoUsuario)
    {
        //  Regra de negócio
        if (tipoUsuario != TipoUsuario.Contador)
            throw new UnauthorizedAccessException("Apenas contadores podem consultar CNPJ");

        //  Limpeza do CNPJ
        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        //  Validação
        if (cnpj.Length != 14)
            throw new Exception("CNPJ inválido");

        //  Consulta API externa
        var result = await _client.ConsultarCnpj(cnpj);

        if (result == null)
            throw new Exception("CNPJ não encontrado");

        return result;
    }
}