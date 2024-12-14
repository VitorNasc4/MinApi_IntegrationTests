using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MinApiLivros.Context;

namespace MotoRental.Test.Integration.Factory;


public class LivrosWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly AppDbContext _sharedDbContext;
    public LivrosWebApplicationFactory(AppDbContext sharedDbContext)
    {
        _sharedDbContext = sharedDbContext;
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Define a variável de ambiente ASPNETCORE_ENVIRONMENT como "Testing".
        // Isso garante que as configurações específicas de testes sejam aplicadas no Program.cs.
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        builder.ConfigureServices(services =>
        {
            // Remove a configuração original do DbContext, que pode ter sido registrada no Program.cs.
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            // Adiciona a instância compartilhada do DbContext (injetada via construtor).
            // Isso garante que o mesmo banco de dados em memória seja reutilizado em todos os testes.
            services.AddSingleton(_sharedDbContext);

            // Registra um novo DbContext configurado para usar um banco de dados em memória.
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));
        });

        // Chama a implementação base para continuar configurando o WebHost padrão.
        base.ConfigureWebHost(builder);
    }

    // Método auxiliar que cria um escopo para acessar o DbContext configurado.
    // Isso é útil quando queremos acessar o DbContext diretamente durante os testes.
    public AppDbContext CreateDbContext()
    {
        // Cria um escopo de serviço para resolver dependências.
        var scope = Services.CreateScope();

        // Obtém a instância configurada do AppDbContext no escopo de serviços.
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}
