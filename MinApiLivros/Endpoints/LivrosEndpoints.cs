using MinApiLivros.Entities;
using MinApiLivros.Services;

namespace MinApiLivros.Endpoints;
public static class LivrosEndpoints
{
    public static void RegisterLivrosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/livros", async (Livro livro, ILivroService _livroService) =>
        {
            await _livroService.AddLivro(livro);
            return Results.Created($"{livro.Id}", livro);
        });

        endpoints.MapGet("/livros", async (ILivroService _livroService) =>
        {
            var livros = await _livroService.GetLivros();
            return Results.Ok(livros);
        });

        endpoints.MapGet("/livros/{id}", async (ILivroService _livroService, int id) =>
        {
            var livro = await _livroService.GetLivro(id);

            if (livro != null)
                return Results.Ok(livro);
            else
                return Results.NotFound();
        });

        endpoints.MapDelete("/livros/{id}", async (int id, ILivroService _livroService) =>
        {
            await _livroService.DeleteLivro(id);
            return Results.Ok($"Livro de id={id} deletado");
        });

        endpoints.MapPut("/livros/{id}", async (int id, Livro livro, ILivroService _livroService) =>
        {
            if (livro is null)
                return Results.BadRequest("Dados inválidos");
            livro.Id = id;
            await _livroService.UpdateLivro(livro);
            return Results.Ok(livro);
        });
    }
}



