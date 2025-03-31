using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
using MinimalAPIConcepts.Services.Repository;
using NEXT.GEN.Models.EmailModel;
using NEXT.GEN.Services.HTTPONLY_MIDDLEWARE;
using NEXT.GEN.Services.Interfaces;
using NEXT.GEN.Services.Repository;
using System.Net;
using System.Net.Mail;
using System.Text;

// google
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Azure.Core;

var builder = WebApplication.CreateBuilder(args);

// Add authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = builder.Configuration["GoogleKeys:ClientId"];
//    googleOptions.ClientSecret = builder.Configuration["GoogleKeys:CLIENTSECRET"];
//    googleOptions.CallbackPath = "/signin-google"; // This is the default value
//});

builder.Services.AddHttpContextAccessor();

// for resolving multiple object cycles
//builder.Services.Configure<JsonOptions>(
//    options =>
//    {
//        options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//        //options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//    });

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
            builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:5173"))
);

// Add Jwt configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//    .AddCookie()
//    .AddGoogle(options =>
//    {
//        var clientId = builder.Configuration["Authentication:Google:ClientId"];

//        // check if the clientId is null
//        if(clientId == null)
//        {
//            throw new ArgumentNullException(nameof(clientId));
//        }

//        var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

//        if(clientSecret == null)
//        {
//            throw new ArgumentNullException(nameof(clientSecret));  
//        }

//        options.ClientId = clientId;
//        options.ClientSecret = clientSecret;
//        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    })
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//});

// add service for use of IdentityUser class 
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // Increase cookie expiration to handle the full OAuth flow
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.Name = "GoogleAuth";
    options.Cookie.HttpOnly = true;

    //the cookie is sent with requests from the same site and during top-level navigations to the cookie's domain
    //from a third-party site, helping to mitigate Cross-Site Request Forgery (CSRF) attacks. 
    options.Cookie.SameSite = SameSiteMode.Lax; 
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.SaveTokens = true;
    options.UsePkce = true; // For OAuth2 providers that support it
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minimal Api Practice", Description = "Say hello minimal api", Version = "v1" });

        //  define the security defination of the added security scheme
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Bearer",
            Type = SecuritySchemeType.Http,
            Description = "Please enter the jwt token here for authorization",
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
             {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
        });
    }
);

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
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ICommentRepository,CommentRepository>();



var app = builder.Build();

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<JwtMiddleware>();
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