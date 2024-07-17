using Serilog;
using UserService.API.Interceptors;
using UserService.API.Mappers;
using UserService.API.Services;
using UserService.Application;
using UserService.Persistance;
using UserService.Persistance.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console().CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options => { options.Interceptors.Add<ServerExceptionsInterceptor>(); });

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddMappers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.MapGrpcService<StudentService>();
app.MapGrpcService<GroupService>();
app.MapGrpcService<TeacherService>();

app.Run();