using System.Text;
using ePasServices.Data;
using ePasServices.Services.Interfaces;
using ePasServices.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ePasServices.Helpers;

var builder = WebApplication.CreateBuilder(args);

//PROD
//builder.WebHost.UseUrls("http://0.0.0.0:5000");

builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IMasterQuestionerService, MasterQuestionerService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITrxAuditService, TrxAuditService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Konfigurasi Swagger supaya bisa pakai Authorization header global
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ePas API", Version = "v1" });

    // Tambahkan definisi Authorization: Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Masukkan token seperti ini: Bearer {your JWT token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<PostgreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]); 
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            Console.WriteLine("Jwt:Key tidak ditemukan, menggunakan default bawaan.");
            jwtKey = "dT9kx4UJm3qzXv8YLnG1WAZfsVQ7HJp0";
        }

        var key = Encoding.UTF8.GetBytes(jwtKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };
    });

var app = builder.Build();

// PROD
//app.UseSwagger();
//app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();