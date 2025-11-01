namespace API_Service___Hidroponico__Invernadero_.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraEn { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string? Rol { get; set; }
    }
}
