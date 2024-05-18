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
// indicamos que la api tendr� autenticaci�n basada en jwt token, para validar usamos los tres par�metros: audience, issuer
// y signing key, se podr�an a�adir m�s propiedades de validaci�n para el token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //.AddJwtBearer(o => o.TokenValidationParameters = new()
    //{
    //    ValidIssuer = "http://localhost:9000",
    //    ValidateIssuer = true,
    //    ValidAudience = "http://localhost:9000",
    //    ValidateAudience = true,
    //    IssuerSigningKey = "",
    //});
    // m�s elegante:
    .AddJwtBearer();

// configuraci�n del jwt
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

// nuestro servicio como transiente para generar el token
builder.Services.AddTransient<IJwtProvider, JwtProvider>();

// a�adimos autorizaci�n adem�s de autenticaci�n (a�n sin implementar)
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// NOTA: este proyecto referencia l�gicamente al de aplicaci�n,
// pero tambi�n a infrastructure, en ese �ltimo caso para poder acceder a DependencyInjection.cs
// aqu� llamamos a la configuraci�n de DI que tenemos en los proyectos de aplicaci�n e infrastructure
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
// tener en cuenta que no har� falta el Update-Database, ya que hemos creado ApplicationBuilderExtensions
// con ApplyMigration, que se encargar� de ejecutar los archivos de migraci�n al arrancar la aplicaci�n
app.ApplyMigration();

// para poblar tablas con datos fake con Bogus
// lo comentamos, porque si no va a insertar lo que tenga el m�todo cada vez que arranque la aplicaci�n
app.SeedData();
app.SeedDataAuthentication();

// a�adimos a la pila de ejecuci�n nuestro custom exception handler
app.UseCustomExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();
