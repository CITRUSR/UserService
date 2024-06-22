using UserService.Application;
using UserService.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();