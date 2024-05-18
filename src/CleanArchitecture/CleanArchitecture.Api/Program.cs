using CleanArchitecture.Api.Extensions;
using CleanArchitecture.Api.OptionsSetup;
using CleanArchitecture.Application;
using CleanArchitecture.Application.Abstractions.Authentication;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// indicamos que la api tendrá autenticación basada en jwt token, para validar usamos los tres parámetros: audience, issuer
// y signing key, se podrían añadir más propiedades de validación para el token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //.AddJwtBearer(o => o.TokenValidationParameters = new()
    //{
    //    ValidIssuer = "http://localhost:9000",
    //    ValidateIssuer = true,
    //    ValidAudience = "http://localhost:9000",
    //    ValidateAudience = true,
    //    IssuerSigningKey = "",
    //});
    // más elegante:
    .AddJwtBearer();

// configuración del jwt
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

// nuestro servicio como transiente para generar el token
builder.Services.AddTransient<IJwtProvider, JwtProvider>();

// añadimos autorización además de autenticación (aún sin implementar)
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// NOTA: este proyecto referencia lógicamente al de aplicación,
// pero también a infrastructure, en ese último caso para poder acceder a DependencyInjection.cs
// aquí llamamos a la configuración de DI que tenemos en los proyectos de aplicación e infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// migraciones
// https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs
// tener en cuenta que no hará falta el Update-Database, ya que hemos creado ApplicationBuilderExtensions
// con ApplyMigration, que se encargará de ejecutar los archivos de migración al arrancar la aplicación
app.ApplyMigration();

// para poblar tablas con datos fake con Bogus
// lo comentamos, porque si no va a insertar lo que tenga el método cada vez que arranque la aplicación
app.SeedData();
app.SeedDataAuthentication();

// añadimos a la pila de ejecución nuestro custom exception handler
app.UseCustomExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();
