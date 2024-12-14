using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinApiLivros.Entities;
using MotoRental.Test.Integration.Factory;

namespace MinApiLivros.Test;

public class LivroIntegrationTest : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    
    private readonly DatabaseFixture _fixture;
    private readonly LivrosWebApplicationFactory _factory;
    public LivroIntegrationTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _factory = new LivrosWebApplicationFactory(_fixture.DbContext);

        _fixture.ClearDatabase();
    }
    public async Task InitializeAsync()
    {
        await _fixture.DbContext.Database.EnsureDeletedAsync();
        await _fixture.DbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task POST_CriandoLivro_OnSucces()
    {
        var dbContext = _fixture.DbContext;
        using var client = _factory.CreateClient();

        var livro = new Livro { Autor = "Autor Teste", Titulo = "Titulo Teste" };

        var result = await client.PostAsJsonAsync("/livros", livro);

        Assert.Equal(HttpStatusCode.Created, result.StatusCode);

        var resultFromDbData = await dbContext.Livros.FirstOrDefaultAsync();
        Assert.NotNull(resultFromDbData);
        Assert.Equal(livro.Autor, resultFromDbData.Autor);
        Assert.Equal(livro.Titulo, resultFromDbData.Titulo);
    }
    [Fact]
    public async Task GET_BuscandoLivroPorId_OnSucces()
    {
        var dbContext = _fixture.DbContext;
        using var client = _factory.CreateClient();

        var livro = new Livro { Id = 1, Autor = "Autor Teste", Titulo = "Titulo Teste" };

        dbContext.Livros.Add(livro);
        await dbContext.SaveChangesAsync();

        var result = await client.GetAsync($"/livros/{livro.Id}");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var resultData = await result.Content.ReadFromJsonAsync<Livro>();
        Assert.NotNull(resultData);
        Assert.Equal(livro.Autor, resultData.Autor);
        Assert.Equal(livro.Titulo, resultData.Titulo);
    }
}