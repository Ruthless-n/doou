using Doou.Api.Config;
using Doou.Api.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using System;

Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

var connectionString = $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
                       $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";

Console.WriteLine(" Verificando variáveis carregadas do .env...");
Console.WriteLine($"POSTGRES_HOST = {Environment.GetEnvironmentVariable("POSTGRES_HOST")}");
Console.WriteLine($"POSTGRES_DB = {Environment.GetEnvironmentVariable("POSTGRES_DB")}");
Console.WriteLine($"POSTGRES_USER = {Environment.GetEnvironmentVariable("POSTGRES_USER")}");

builder.Services.AddDbContext<DoouDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
