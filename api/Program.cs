using api.Data;
using api.Models;
using api.Repositories;
using api.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// cors
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .Build());
});

// ioc
services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(databaseName: "Test"));

services.AddScoped<DataSeeder>();
services.AddScoped<IClientRepository, ClientRepository>();
services.AddScoped<IValidator<Client>, ClientValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/clients", async (IClientRepository clientRepository) =>
{
    return await clientRepository.Get();
})
.WithName("get clients");

app.MapGet("/clients/{name}", async (IClientRepository clientRepository, string name) =>
{
    return await clientRepository.Search(name);
})
.WithName("search clients");


app.MapPost("/clients", async (IClientRepository clientRepository, IValidator<Client> validator, Client client) =>
{
    var valResult = validator.Validate(client);

    if (valResult.IsValid)
    {
        return await clientRepository.Create(client);
    }
    else
    {
        return Results.ValidationProblem(valResult.ToDictionary());
    }
})
.WithName("create client");

app.MapPut("/clients", async (IClientRepository clientRepository, IValidator<Client> validator, Client client) =>
{
    var valResult = validator.Validate(client);
    if (valResult.IsValid)
    {
        return await clientRepository.Update(client);
    }
    else
    {
        return Results.ValidationProblem(valResult.ToDictionary());
    }
})
.WithName("update client");

app.UseCors();

// seed data
using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();

    dataSeeder.Seed();
}

// run app
app.Run();