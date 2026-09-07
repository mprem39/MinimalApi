using FluentValidation;
using FluentValidation.Results;
using Library.Api;
using Library.Api.Auth;
using Library.Api.Data;
using Library.Api.Endpoints;
using Library.Api.Endpoints.Internal;
using Library.Api.Models;
using Library.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // WebRootPath = "./wwwroot",
    // EnvironmentName = Environment.GetEnvironmentVariable("env"),
    // ApplicationName = "Library.Api"
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.IncludeFields = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigin", x => x.AllowAnyOrigin());
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
{
    // Read the connection string from the Database:ConnectionString key
    var connectionString = builder.Configuration.GetValue<string>("Database:ConnectionString");
    return new SqliteConnectionFactory(connectionString);
});
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddEndpoints<Program>(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);
builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstants.SchemeName, _ => { });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.UseEndpoints<Program>();

//DB init 
var dbInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await dbInitializer.InitializeAsync();

app.Run();


