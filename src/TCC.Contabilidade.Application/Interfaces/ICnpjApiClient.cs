using TCC.Contabilidade.Application.DTO;

namespace TCC.Contabilidade.Application.Interfaces;

public interface ICnpjApiClient
{
    Task<CnpjResponseDTO?> ConsultarCnpj(string cnpj);
}