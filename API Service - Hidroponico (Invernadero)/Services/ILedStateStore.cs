using API_Service___Hidroponico__Invernadero_.Models;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    /// <summary>
    /// Almacén en memoria: estado de actuadores y lecturas DHT.
    /// </summary>
    public interface ILedStateStore
    {
        // LEDs (1=Luces, 2=Bomba, 3=Ventilador)
        bool Get(int id);
        bool Set(int id, bool on);
        bool Toggle(int id);

        // DHT11
        void AddDht(DhtReading reading);
        DhtReading? GetLastDht();
        IEnumerable<DhtReading> GetDhtHistory(int take = 50);
    }
}
