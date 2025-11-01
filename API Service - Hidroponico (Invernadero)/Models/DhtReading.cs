namespace API_Service___Hidroponico__Invernadero_.Models
{
    public class DhtReading
    {
        public DateTime Utc { get; set; }        // timestamp UTC
        public float TemperatureC { get; set; }  // °C
        public float Humidity { get; set; }      // %
        public string? Source { get; set; }      // opcional (ej. "esp32")
    }
}
