namespace API_Service___Hidroponico__Invernadero_.DTOs
{
    public class LedDtos
    {
        // Para LEDs
        public record SetLedStateDto(bool On);

        // Para DHT11
        public record DhtInDto(float temperatureC, float humidity, string? source);
        public record DhtOutDto(DateTime utc, float temperatureC, float humidity, string? source);
    }
}
