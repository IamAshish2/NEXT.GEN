using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
using MinimalAPIConcepts.Services.Repository;
using NEXT.GEN.Models.EmailModel;
using NEXT.GEN.Services.Interfaces;
using NEXT.GEN.Services.Repository;
using System.Net;
using System.Net.Mail;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add auto mapper configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add fluent email service to the application
var smtp = builder.Configuration.GetSection("Smtp").Get<SmtpSettings>();
builder.Services.AddFluentEmail(smtp.FromEmail,smtp.FromName)
    .AddSmtpSender(new SmtpClient(smtp.Host)
    {
        Port = smtp.Port,
        Credentials =  new NetworkCredential(smtp.FromEmail,smtp.Password),
        EnableSsl = smtp.EnableSsl,
    });

// adding the cors policy for all origins default.
builder.Services.AddCors( options => 
            options.AddPolicy("AllowAllOrigins",
            builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())            
);

// Add Jwt configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minimal Api Practice", Description = "Say hello minimal api", Version = "v1" });
});

// Register controllers
builder.Services.AddControllers();
//.AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//});

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

// Dependency injection for repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenGenerator,TokenGenerator>();
builder.Services.AddScoped<IEmailService,EmailService>();
builder.Services.AddScoped<IFriendshipsRepository,FriendshipRepository>();
builder.Services.AddScoped<IpostRepository, PostRepository>();
builder.Services.AddScoped<IGroupRepository,GroupRepository>();
builder.Services.AddScoped<IpostRepository,PostRepository>();
builder.Services.AddScoped<IGroupMemberRepository,GroupMemberRepository>();



var app = builder.Build();

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
//app.UseAuthorization();

// Map controllers
app.MapControllers();

// Swagger configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal Api Practice v1");
    });
}

app.Run();