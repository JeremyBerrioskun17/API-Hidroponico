using API_Service___Hidroponico__Invernadero_.Models;
using System.Collections.Concurrent;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    /// <summary>
    /// Implementación en memoria, sin base de datos.
    /// </summary>
    public class MemoryLedStateStore : ILedStateStore
    {
        private readonly ConcurrentDictionary<int, bool> _map = new();

        // Buffer DHT (simple)
        private readonly object _lock = new();
        private readonly List<DhtReading> _dht = new();
        private const int MaxDht = 1000;

        public MemoryLedStateStore()
        {
            _map[1] = false; // Luces
            _map[2] = false; // Bomba
            _map[3] = false; // Ventilador
        }

        // ===== LEDs =====
        public bool Get(int id) => _map.TryGetValue(id, out var v) ? v : false;

        public bool Set(int id, bool on)
        {
            _map[id] = on;
            return on;
        }

        public bool Toggle(int id)
        {
            var now = Get(id);
            _map[id] = !now;
            return !now;
        }

        // ===== DHT =====
        public void AddDht(DhtReading reading)
        {
            lock (_lock)
            {
                _dht.Add(reading);
                if (_dht.Count > MaxDht) _dht.RemoveAt(0);
            }
        }

        public DhtReading? GetLastDht()
        {
            lock (_lock)
            {
                return _dht.Count == 0 ? null : _dht[^1];
            }
        }

        public IEnumerable<DhtReading> GetDhtHistory(int take = 50)
        {
            lock (_lock)
            {
                return _dht.TakeLast(take).ToArray();
            }
        }
    }
}
