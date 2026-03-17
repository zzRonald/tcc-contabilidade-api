using System.Text.RegularExpressions;
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

    public async Task<Convite?> ObterConvitePorTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("Token do convite não informado.");

        return await _conviteRepository.ObterPorTokenAsync(token);
    }

    public async Task<string> CriarConviteAsync(string emailCliente, Guid contadorId)
    {
        // 🔥 VALIDAÇÃO DE ENTRADA
        if (string.IsNullOrWhiteSpace(emailCliente))
            throw new Exception("É necessário informar um email para enviar o convite.");

        if (!IsValidEmail(emailCliente))
            throw new Exception("O email informado não é válido.");

        if (contadorId == Guid.Empty)
            throw new Exception("Identificador do usuário inválido.");

        // 🔥 REGRA DE NEGÓCIO (evitar duplicidade ativa)
        var convitesExistentes = await _conviteRepository.GetByContadorId(contadorId);

        var conviteAtivo = convitesExistentes
            .FirstOrDefault(c =>
                c.EmailCliente == emailCliente &&
                !c.Usado &&
                c.Expiracao > DateTime.UtcNow
            );

        if (conviteAtivo != null)
            throw new Exception("Já existe um convite ativo para este email.");

        // 🔥 CRIAÇÃO SEGURA
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

    public async Task<List<Convite>> GetConvitesByContadorIdAsync(Guid contadorId)
    {
        if (contadorId == Guid.Empty)
            throw new Exception("Identificador do usuário inválido.");

        return await _conviteRepository.GetByContadorId(contadorId);
    }

    // 🔥 MÉTODO AUXILIAR (validação real de email)
    private bool IsValidEmail(string email)
    {
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }
}