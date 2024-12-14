using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MinApiLivros.Context;

namespace MotoRental.Test.Integration.Factory;
public class DatabaseFixture : IDisposable
{
    public AppDbContext DbContext { get; private set; }
    private readonly InMemoryDatabaseRoot _databaseRoot;

    public DatabaseFixture()
    {
        // Cria uma raiz de banco de dados em memória compartilhada
        // para permitir que a mesma instância seja reutilizada entre os testes.
        _databaseRoot = new InMemoryDatabaseRoot();

        // Configura o DbContext para usar um banco de dados em memória.
        // "TestDatabase" é o nome do banco, e "_databaseRoot" garante que
        // o banco seja consistente entre as execuções.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase", _databaseRoot)
            .Options;

        // Inicializa o DbContext com as opções configuradas.
        DbContext = new AppDbContext(options);

        // Garante que o banco de dados seja criado no início.
        DbContext.Database.EnsureCreated();
    }


    public void ClearDatabase()
    {
        // Para cada tabela do banco
        DbContext.Livros.RemoveRange(DbContext.Livros);
        
        // Ex: se houver uma tabela de autores:
        // DbContext.Autores.RemoveRange(DbContext.Autores);

        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        // Libera os recursos do DbContext
        DbContext.Dispose();
    }
}
