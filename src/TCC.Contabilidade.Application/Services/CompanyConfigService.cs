using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class CompanyConfigService
{
    private readonly ICompanyConfigRepository _repository;
    private readonly IEmpresaRepository _empresaRepository;

    public CompanyConfigService(
        ICompanyConfigRepository repository,
        IEmpresaRepository empresaRepository)
    {
        _repository = repository;
        _empresaRepository = empresaRepository;
    }

    public async Task<CompanyConfigDto> GetByEmpresaId(Guid empresaId, Guid usuarioId)
    {
        await ValidarAcesso(empresaId, usuarioId);

        var config = await _repository.GetByEmpresaIdAsync(empresaId);

        if (config == null)
        {
            config = new CompanyConfig
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId
            };
            await _repository.AddAsync(config);
        }

        return MapToDto(config);
    }

    public async Task Update(Guid empresaId, CompanyConfigDto dto, Guid usuarioId)
    {
        await ValidarAcesso(empresaId, usuarioId);

        var config = await _repository.GetByEmpresaIdAsync(empresaId);

        if (config == null)
        {
            config = new CompanyConfig
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId
            };
            UpdateFromDto(config, dto);
            await _repository.AddAsync(config);
        }
        else
        {
            UpdateFromDto(config, dto);
            await _repository.UpdateAsync(config);
        }
    }

    private async Task ValidarAcesso(Guid empresaId, Guid usuarioId)
    {
        if (!await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, empresaId))
            throw new UnauthorizedAccessException("Usuário não tem acesso a esta empresa.");
    }

    private static CompanyConfigDto MapToDto(CompanyConfig config)
    {
        return new CompanyConfigDto
        {
            MoedaPadrao = config.MoedaPadrao,
            FormatoData = config.FormatoData,
            Timezone = config.Timezone,
            NotificacoesEmail = config.NotificacoesEmail
        };
    }

    private static void UpdateFromDto(CompanyConfig config, CompanyConfigDto dto)
    {
        config.MoedaPadrao = dto.MoedaPadrao;
        config.FormatoData = dto.FormatoData;
        config.Timezone = dto.Timezone;
        config.NotificacoesEmail = dto.NotificacoesEmail;
    }
}
