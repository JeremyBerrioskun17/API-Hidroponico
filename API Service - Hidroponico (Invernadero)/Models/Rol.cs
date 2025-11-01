namespace API_Service___Hidroponico__Invernadero_.Models
{
    public class Rol
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // 'admin','agronomo','inspector','visor'
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
