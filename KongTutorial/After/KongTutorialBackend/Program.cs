using KongTutorialBackend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new DataStore());
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
