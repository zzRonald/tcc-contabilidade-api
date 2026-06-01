using TCC.Contabilidade.Application.DTO;
using TCC.Contabilidade.Application.DTO.Convites;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class ConviteService
{
    private readonly IConviteRepository _conviteRepository;
    private readonly AuditService _auditService;

    public ConviteService(IConviteRepository conviteRepository, AuditService auditService)
    {
        _conviteRepository = conviteRepository;
        _auditService = auditService;
    }

    public async Task<Convite?> ObterConvitePorTokenAsync(string token)
    {
        return await _conviteRepository.ObterPorTokenAsync(token);
    }

    public async Task<string> CriarConviteAsync(string emailCliente, Guid contadorId)
    {
        var token = Guid.NewGuid().ToString();

        var convite = new Convite
        {
            Id = Guid.NewGuid(),
            EmailCliente = emailCliente,
            Token = token,
            ContadorId = contadorId,
            Expiracao = DateTime.UtcNow.AddDays(2),
            Usado = false
        };

        await _conviteRepository.AdicionarAsync(convite);
        await _conviteRepository.SalvarAlteracoesAsync();

        await _auditService.RegistrarEvento("ENVIO_CONVITE", "Convite", convite.Id.ToString(), contadorId);

        return token;
    }

    public async Task<List<Convite>> GetConvitesByContadorIdAsync(Guid contadorId)
    {
        return await _conviteRepository.GetByContadorId(contadorId);
    }

    public async Task<(List<ConviteResponseDto> Items, PaginationMetadataDTO Metadata)> GetPagedConvitesByContadorIdAsync(Guid contadorId, int page, int pageSize)
    {
        var (convites, totalCount) = await _conviteRepository.GetPagedByContadorId(contadorId, page, pageSize);

        var items = convites.Select(c => new ConviteResponseDto
        {
            Token = c.Token,
            EmailDestino = c.EmailCliente,
            DataExpiracao = c.Expiracao,
            Utilizado = c.Utilizado
        }).ToList();

        var metadata = new PaginationMetadataDTO
        {
            PaginaAtual = page,
            TamanhoPagina = pageSize,
            TotalRegistros = totalCount,
            TotalPaginas = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return (items, metadata);
    }
}
