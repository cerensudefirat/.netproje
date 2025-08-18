using api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using api.Interfaces;
using api.Repository;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using api.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers (+ Newtonsoft)
builder.Services.AddControllers()
    .AddNewtonsoftJson(o =>
        o.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Swagger + JWT auth butonu
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

// DI
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();

// DbContext
builder.Services.AddDbContext<ApplicationDB>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDB>()
.AddDefaultTokenProviders();

// Auth/JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        var key = builder.Configuration["Jwt:SigninKey"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

var app = builder.Build();

// ➜ Geçici olarak HTTPS yönlendirmesini kapatmak istersen alttaki satırı yorumlayabilirsin.
// app.UseHttpsRedirection();

// Swagger’ı HER ZAMAN aç (debug kolaylığı için). İstersen dev ile sınırlandırabilirsin.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";                     // /swagger
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
