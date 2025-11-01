using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokenService;
        private readonly IPasswordHasher<object> _hasher;

        public AuthController(AppDbContext db, TokenService tokenService, IPasswordHasher<object> hasher)
        {
            _db = db; _tokenService = tokenService; _hasher = hasher;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.UsuarioOCorreo) || string.IsNullOrWhiteSpace(req.Contrasena))
                return BadRequest("Usuario/correo y contraseña son requeridos.");

            var usuario = await _db.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Username == req.UsuarioOCorreo || u.Correo == req.UsuarioOCorreo);

            if (usuario is null || !usuario.Activo)
                return Unauthorized("Credenciales inválidas.");

            // Verificación contra el HASH almacenado en Usuarios.Contrasena
            var result = _hasher.VerifyHashedPassword(new object(), usuario.Contrasena, req.Contrasena);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Credenciales inválidas.");

            var (token, expira) = _tokenService.CrearToken(usuario);

            return Ok(new LoginResponse
            {
                Token = token,
                ExpiraEn = expira,
                Usuario = usuario.Username,
                Rol = usuario.Rol?.Nombre
            });
        }
    }
}
