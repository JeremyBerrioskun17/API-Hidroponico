using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.Services;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ----- Certificado (opcional, para HTTPS React) -----
var kestrelCertPath = builder.Configuration["Kestrel:Certificates:Default:Path"];
var kestrelCertPassword = builder.Configuration["Kestrel:Certificates:Default:Password"];

// Kestrel: HTTP:5000 (ESP32) + HTTPS:7001 (React)
builder.WebHost.ConfigureKestrel(o =>
{
    // HTTP: ESP32 
    o.Listen(IPAddress.Any, 5000, lo => lo.Protocols = HttpProtocols.Http1);

    // HTTPS: React / navegador
    o.Listen(IPAddress.Any, 7001, lo =>
    {
        if (!string.IsNullOrEmpty(kestrelCertPath))
            lo.UseHttps(kestrelCertPath, kestrelCertPassword);
        else
            lo.UseHttps(); // certificado dev
        lo.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

// ----- DbContext -----
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// ----- CORS -----
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEnd", p => p
        .WithOrigins(
            "http://localhost:5173",
            "http://localhost:5174",
            "https://localhost:5173",
            "https://localhost:5174"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
});

// ----- Hasher -----
builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();

// ----- JWT -----
var jwt = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = key,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization();

// ----- Controllers + Swagger -----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// ----- Servicios propios -----
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();
builder.Services.AddSingleton<ILedStateStore, MemoryLedStateStore>();
builder.Services.AddScoped<IHidroponicoService, HidroponicoService>();
builder.Services.AddScoped<ICosechaService, CosechaService>();

var app = builder.Build();

// ----- Swagger en dev -----
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

// ----- Middleware -----
app.UseRouting();
app.UseCors("FrontEnd");

// !! Importante: dejamos HTTP:5000 accesible para ESP32, no redirigir !!
app.UseAuthentication();
app.UseAuthorization();

// ----- Controllers -----
app.MapControllers();

app.Run();
