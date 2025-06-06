using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
// google
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NEXT.GEN.Context;
using NEXT.GEN.Models;
using NEXT.GEN.Models.EmailModel;
using NEXT.GEN.Services.Interfaces;
using NEXT.GEN.Services.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();


// for resolving multiple object cycles
/**
builder.Services.Configure<JsonOptions>(
    options =>
    {
        options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        //options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
*/
// Add auto mapper configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add fluent email service to the application
var smtp = builder.Configuration.GetSection("Smtp").Get<SmtpSettings>();
builder.Services.AddFluentEmail(smtp.FromEmail, smtp.FromName)
    .AddSmtpSender(new SmtpClient(smtp.Host)
    {
        Port = smtp.Port,
        Credentials = new NetworkCredential(smtp.FromEmail, smtp.Password),
        EnableSsl = true,
    });

// adding the cors policy for all origins default.
builder.Services.AddCors(options =>
            options.AddPolicy("AllowAllOrigins",
            builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:5173"))
);

// Add Jwt configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
// Add smtp configuration binding
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

/* previous configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(options =>
    {
        var clientId = builder.Configuration["Authentication:Google:ClientId"];

        // check if the clientId is null
        if (clientId == null)
        {
            throw new ArgumentNullException(nameof(clientId));
        }

        var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        if (clientSecret == null)
        {
            throw new ArgumentNullException(nameof(clientSecret));
        }

        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
*/

// add service for use of IdentityUser class 
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

})
.AddJwtBearer(options =>
{
    // get the jwt settings from appsettings.json and map it to JwtSettings model
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidAudience = jwtSettings?.Audience,
        ValidateAudience = true,

        ValidIssuer = jwtSettings?.Issuer,
        ValidateIssuer = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key ?? "")),
        ValidateIssuerSigningKey = true,

        ClockSkew = TimeSpan.Zero,
    };

    options.Events.OnMessageReceived = context =>
    {
        if (context.Request.Cookies.ContainsKey("auth_token") || context.Request.Cookies.ContainsKey("refresh_token"))
        {
            context.Token = context.Request.Cookies["auth_token"];
            //context. = context.Response.Cookies["refresh_token"];
        }
        ;
        return Task.CompletedTask;
    };
})
.AddCookie(options =>
{
    // Increase cookie expiration to handle the full OAuth flow
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.Name = "GoogleAuth";
    options.Cookie.HttpOnly = true;
    //the cookie is sent with requests from the same site and during top-level navigations to the cookie's domain
    //from a third-party site, helping to mitigate Cross-Site Request Forgery (CSRF) attacks. 
    options.Cookie.SameSite = SameSiteMode.None;
    options.LoginPath = "/api/auth/login";
    options.LogoutPath = "/api/auth/logout";
})
.AddGoogle(options =>
{
    // works only on local development as i set it with dotnet secret manager
    //options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    //options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    var google = builder.Configuration.GetSection("GoogleKeys");
    
    options.ClientId = google["ClientId"];
    options.ClientSecret = google["ClientSecret"];
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
                    },
                },
                new string[]{}
             },
        });
}
);

// Register controllers
builder.Services.AddControllers();
//.AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//});

// Database connection with azure sql
//var sqlConnection = builder.Configuration["ConnectionStrings:SqlDb"];
//builder.Services.AddSqlServer<ApplicationDbContext>(sqlConnection, options => options.EnableRetryOnFailure());

// Local database connection with mssql
var sqlConnection = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(sqlConnection));

// Dependency injection for repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFriendshipsRepository, FriendshipRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddSingleton<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IUserProfilePictureRepository, UserProfileImageRepository>();
builder.Services.AddScoped<ICloudinaryUploadRepository, CloudinaryUploadRepository>();


var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
// app.UseAuthorization();
app.UseHttpsRedirection();

// TODO -> TRY USING THIS MIDDLEWARE EVEN BEFORE HITTING THE CONTROLLERS 
// but the same work is being done by the authentication middleware , the token validation parameters
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
        //c.RoutePrefix = string.Empty;
    });
}

app.Run();