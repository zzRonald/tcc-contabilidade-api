using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TCC.Contabilidade.Infrastructure.Data;
using TCC.Contabilidade.Application.Interfaces;
using Moq;
using TCC.Contabilidade.Infrastructure.Integrations;

namespace TCC.Contabilidade.Tests;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    public Mock<IEmailService> EmailServiceMock { get; } = new();
    public Mock<ICnpjApiClient> CnpjApiClientMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            // Replace external services with mocks
            services.Remove(services.Single(d => d.ServiceType == typeof(IEmailService)));
            services.AddScoped(_ => EmailServiceMock.Object);

            services.Remove(services.Single(d => d.ServiceType == typeof(ICnpjApiClient)));
            services.AddScoped(_ => CnpjApiClientMock.Object);
        });
    }
}
