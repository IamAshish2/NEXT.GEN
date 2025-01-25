using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Interfaces;
using MinimalAPIConcepts.Repository;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
// Add the Swagger Service to the application
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minimal Api Practice", Description = "Say hello minimal api", Version = "v1" });
});

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });


// Creating the connection to the database
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddScoped<IUserRepository,UserRepository>();

var app = builder.Build();

app.UseHttpsRedirection();
 
// map the controllers so when you hit a endpoint it actually works
app.MapControllers();


// actually use the swagger ui
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
    c.SwaggerEndpoint("/swagger/v1/swagger.json","Minimal Api Practice v1");
    });
}


app.Run();