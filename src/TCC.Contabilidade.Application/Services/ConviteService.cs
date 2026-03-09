using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Domain.Entities;

namespace TCC.Contabilidade.Application.Services;

public class ConviteService
{
    private readonly IConviteRepository _conviteRepository;

    public ConviteService(IConviteRepository conviteRepository)
    {
        _conviteRepository = conviteRepository;
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

        return token;
    }
}