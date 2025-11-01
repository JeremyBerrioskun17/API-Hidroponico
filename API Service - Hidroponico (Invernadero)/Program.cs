//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using API_Service___Hidroponico__Invernadero_.Data;
//using API_Service___Hidroponico__Invernadero_.Services;
//using System.Text.Json.Serialization;

//var builder = WebApplication.CreateBuilder(args);


//// ====== Kestrel: HTTP:5000 (ESP32) + HTTPS:7001 (web) ======
//builder.WebHost.ConfigureKestrel(o =>
//{
//    o.ListenAnyIP(5000);                      // HTTP (para ESP32 y Postman desde IP)
//    o.ListenAnyIP(7001, lo => lo.UseHttps()); // HTTPS (para navegador, opcional)
//});

//// Acepta conexiones desde cualquier interfaz de red en 5000 (HTTP) y 7001 (HTTPS)
//builder.WebHost.UseUrls("http://0.0.0.0:5000;https://0.0.0.0:7001");


//// ========================
//// DbContext (SQL Server)
//// ========================
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

//// ========================
//// CORS (FrontEnd en Vite)
//// ========================
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("FrontEnd", p => p
//        .WithOrigins("http://localhost:5173")
//        .AllowAnyHeader()
//        .AllowAnyMethod()
//    );
//});

//// ========================
//// Hasher (sin Identity completo)
//// ========================
//builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();

//// ========================
//// JWT
//// ========================
//var jwt = builder.Configuration.GetSection("Jwt");
//var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

//builder.Services.AddAuthentication(opt =>
//{
//    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(opt =>
//{
//    opt.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwt["Issuer"],
//        ValidAudience = jwt["Audience"],
//        IssuerSigningKey = key,
//        ClockSkew = TimeSpan.FromSeconds(30)
//    };
//});

//builder.Services.AddAuthorization();

//// ========================
//// Swagger + Controllers
//// ========================
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddControllers()
//    .AddJsonOptions(o =>
//        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); // enums como string

//// ========================
//// Servicios propios
//// ========================
//builder.Services.AddSingleton<TokenService>();
//builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();
////builder.Services.AddSingleton<ILedService, LedService>();

//// Estado LED EN MEMORIA (sin DB)
//builder.Services.AddSingleton<ILedStateStore, MemoryLedStateStore>();


//// === Opciones Serial desde appsettings ===
//builder.Services.Configure<SerialOptions>(builder.Configuration.GetSection("Serial"));

//var app = builder.Build();

//// ========================
//// Middleware
//// ========================
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

////app.UseHttpsRedirection();
//app.UseCors();
//app.UseCors("FrontEnd");

//app.UseAuthentication();
//app.UseAuthorization();

//// ========================
//// Controllers
//// ========================
//app.MapControllers();

//// ========================
//// Run
//// ========================
//app.Run();

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

var builder = WebApplication.CreateBuilder(args);

// Kestrel: HTTP:5000 (ESP32/Postman) + HTTPS:7001 (web)
builder.WebHost.ConfigureKestrel(o =>
{
    o.Listen(IPAddress.Any, 5000, lo => lo.Protocols = HttpProtocols.Http1);
    o.Listen(IPAddress.Any, 7001, lo => { lo.UseHttps(); lo.Protocols = HttpProtocols.Http1AndHttp2; });
});

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// CORS para Vite
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEnd", p => p
        .WithOrigins("http://localhost:5173")   // agrega también https si algún día sirves Vite con TLS
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()                     // porque el front usa credentials: "include"
    );
});

// Hasher
builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();

// JWT
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

// Swagger + Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Servicios propios
builder.Services.AddSingleton<TokenService>();
builder.Services.AddScoped<IDiagnosticoService, DiagnosticoService>();
builder.Services.AddSingleton<ILedStateStore, MemoryLedStateStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// OJO: orden recomendado
app.UseRouting();
app.UseCors("FrontEnd");
// app.UseHttpsRedirection(); // déjalo off si el ESP32 usa http:5000
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
