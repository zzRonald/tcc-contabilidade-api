using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TCC.Contabilidade.Application.Interfaces;
using TCC.Contabilidade.Infrastructure.Data;

namespace TCC.Contabilidade.Tests.Fixtures;

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
                options.UseInMemoryDatabase("IntegrationTestsDb");
            });

            // Replace external services with mocks
            var emailServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (emailServiceDescriptor != null) services.Remove(emailServiceDescriptor);
            services.AddScoped(_ => EmailServiceMock.Object);

            var cnpjServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICnpjApiClient));
            if (cnpjServiceDescriptor != null) services.Remove(cnpjServiceDescriptor);
            services.AddScoped(_ => CnpjApiClientMock.Object);
        });
    }

    public void ResetDatabase()
    {
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
