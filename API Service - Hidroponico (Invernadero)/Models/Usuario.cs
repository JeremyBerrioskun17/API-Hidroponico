namespace API_Service___Hidroponico__Invernadero_.Models
{
    public class Usuario
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        // IMPORTANTE: aquí guardas el HASH de la contraseña (no texto plano)
        public string Contrasena { get; set; } = string.Empty;

        public string? Foto { get; set; }
        public long RolId { get; set; }
        public Rol? Rol { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
