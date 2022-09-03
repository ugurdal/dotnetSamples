using FluentValidation;
using MinimalAPIs.Infrastructure;
using MinimalAPIs.Models;
using static Microsoft.AspNetCore.Http.Results;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/person", (Validated<Person> req) =>
{
    var (isValid, value) = req;

    return isValid
        ? Ok(value)
        : ValidationProblem(req.Errors);
});

app.Run();