using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class CompanyConfigService
{
    private readonly ICompanyConfigRepository _repository;
    private readonly IEmpresaRepository _empresaRepository;

    public CompanyConfigService(ICompanyConfigRepository repository, IEmpresaRepository empresaRepository)
    {
        _repository = repository;
        _empresaRepository = empresaRepository;
    }

    public async Task<CompanyConfigDTO> GetByEmpresaId(Guid empresaId, Guid usuarioId)
    {
        await ValidateUserAccess(empresaId, usuarioId);

        var config = await _repository.GetByEmpresaId(empresaId);

        if (config == null)
        {
            return new CompanyConfigDTO
            {
                EmpresaId = empresaId,
                MoedaPadrao = "BRL",
                FormatoData = "dd/MM/yyyy",
                Timezone = "America/Sao_Paulo",
                NotificacoesEmail = true
            };
        }

        return MapToDto(config);
    }

    public async Task Upsert(CompanyConfigDTO dto, Guid usuarioId)
    {
        await ValidateUserAccess(dto.EmpresaId, usuarioId);

        var config = await _repository.GetByEmpresaId(dto.EmpresaId);

        if (config == null)
        {
            config = new CompanyConfig
            {
                Id = Guid.NewGuid(),
                EmpresaId = dto.EmpresaId,
                MoedaPadrao = dto.MoedaPadrao,
                FormatoData = dto.FormatoData,
                Timezone = dto.Timezone,
                NotificacoesEmail = dto.NotificacoesEmail
            };
            await _repository.AddAsync(config);
        }
        else
        {
            config.MoedaPadrao = dto.MoedaPadrao;
            config.FormatoData = dto.FormatoData;
            config.Timezone = dto.Timezone;
            config.NotificacoesEmail = dto.NotificacoesEmail;
            await _repository.UpdateAsync(config);
        }
    }

    private async Task ValidateUserAccess(Guid empresaId, Guid usuarioId)
    {
        if (!await _empresaRepository.IsUsuarioVinculadoAsync(usuarioId, empresaId))
        {
            throw new UnauthorizedAccessException("Usuário não tem permissão para acessar as configurações desta empresa.");
        }
    }

    private CompanyConfigDTO MapToDto(CompanyConfig config)
    {
        return new CompanyConfigDTO
        {
            EmpresaId = config.EmpresaId,
            MoedaPadrao = config.MoedaPadrao,
            FormatoData = config.FormatoData,
            Timezone = config.Timezone,
            NotificacoesEmail = config.NotificacoesEmail
        };
    }
}
