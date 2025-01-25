using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen((c) =>
    {
    c.SwaggerDoc("v1",new OpenApiInfo { Title = "Minimal Api Practice", Description = "Say hello minimal api", Version = "v1"});
    });

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
    c.SwaggerEndpoint("/swagger/v1/swagger.json","Minimal Api Practice v1");
    });
}

app.MapGet("/", () => "Hello World");

app.Run();