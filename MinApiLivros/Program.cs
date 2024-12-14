using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinApiLivros.Context;
using MinApiLivros.Endpoints;
using MinApiLivros.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ILivroService, LivroService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    string ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

    if (ENVIRONMENT == "Testing") 
    {
        // Usa banco em memória apenas no ambiente de teste
        options.UseInMemoryDatabase("MotoRentalDatabase");
    }
    else
    {
        // Use seu banco de preferência em outros ambientes
        options.UseInMemoryDatabase("AppDbInMemory");
    }
});


var app = builder.Build();

app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterLivrosEndpoints();

app.Run();

public partial class Program { }