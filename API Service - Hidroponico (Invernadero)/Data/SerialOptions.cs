namespace API_Service___Hidroponico__Invernadero_.Data
{
    // ===== Opciones serial (POCO) =====
    public class SerialOptions
    {
        public string Port { get; set; } = "COM4";
        public int Baud { get; set; } = 115200;
        public int ReadTimeoutMs { get; set; } = 800;
        public int WriteTimeoutMs { get; set; } = 800;
        public bool DtrEnable { get; set; } = false; // cambia a true si lo necesitas
        public bool RtsEnable { get; set; } = false; // cambia a true si lo necesitas
    }
}
