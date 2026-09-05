using Library.Api.Data;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
{
    // Read the connection string from the Database:ConnectionString key
    var connectionString = builder.Configuration.GetValue<string>("Database:ConnectionString");
    return new SqliteConnectionFactory(connectionString);
});
builder.Services.AddSingleton<DatabaseInitializer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//DB init 
var dbInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await dbInitializer.InitializeAsync();

app.Run();


