using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Infrastructure.Services;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        // Mock implementation: just log to console
        Console.WriteLine("========================================");
        Console.WriteLine($"MOCK EMAIL SENT TO: {to}");
        Console.WriteLine($"SUBJECT: {subject}");
        Console.WriteLine($"BODY: {body}");
        Console.WriteLine("========================================");

        return Task.CompletedTask;
    }
}
