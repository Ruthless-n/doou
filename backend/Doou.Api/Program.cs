using Doou.Api.Config;
using Doou.Api.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

var connectionString = $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
                       $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";

var secretKey = builder.Configuration["JWT_SECRET"] ?? throw new InvalidOperationException("JWT_SECRET não está configurado.");
var issuer = builder.Configuration["JWT_ISSUER"] ?? throw new InvalidOperationException("JWT_ISSUER não está configurado.");
var audience = builder.Configuration["JWT_AUDIENCE"] ?? throw new InvalidOperationException("JWT_AUDIENCE não está configurado.");

builder.Services.AddDbContext<DoouDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
   options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
    };
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Doou API",
        Version = "v1",
        Description = "API para o Aplicativo Doou - doações por proximidade"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira 'Bearer' seguido do token JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();


var app = builder.Build();

builder.Services.AddAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
