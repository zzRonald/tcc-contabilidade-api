using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Domain.Enums;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Application.Services;

public class CnpjService
{
    private readonly ICnpjApiClient _client;
    private readonly CacheService _cache;
    private const string CachePrefix = "cnpj";

    public CnpjService(ICnpjApiClient client, CacheService cache)
    {
        _client = client;
        _cache = cache;
    }

    public async Task<CnpjResponseDTO> ConsultarCnpj(string cnpj, TipoUsuario tipoUsuario)
    {
        //  Regra de negócio
        if (tipoUsuario != TipoUsuario.Contador && tipoUsuario != TipoUsuario.Admin)
            throw new UnauthorizedAccessException("Apenas contadores e administradores podem consultar CNPJ");

        //  Limpeza do CNPJ
        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        //  Validação
        if (cnpj.Length != 14)
            throw new Exception("CNPJ inválido");

        string cacheKey = _cache.GenerateKey(CachePrefix, cnpj);

        //  Consulta API externa com cache
        var result = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            return await _client.ConsultarCnpj(cnpj);
        }, TimeSpan.FromHours(2));

        if (result == null)
            throw new Exception("CNPJ não encontrado");

        return result;
    }
}
