using API_Service___Hidroponico__Invernadero_.Models;
using API_Service___Hidroponico__Invernadero_.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static API_Service___Hidroponico__Invernadero_.DTOs.LedDtos;

namespace API_Service___Hidroponico__Invernadero_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LedsController : ControllerBase
    {
        private readonly ILedStateStore _store;
        public LedsController(ILedStateStore store) => _store = store;

        // ===== LEDs =====
        public record LedStateDto(bool On);

        [HttpGet("{id:int}/state")]
        [AllowAnonymous]
        public ActionResult<LedStateDto> GetState([FromRoute] int id)
            => Ok(new LedStateDto(_store.Get(id)));

        [HttpPut("{id:int}/state")]
        [AllowAnonymous]
        public ActionResult<LedStateDto> SetState([FromRoute] int id, [FromBody] SetLedStateDto body)
            => Ok(new LedStateDto(_store.Set(id, body.On)));

        [HttpPost("{id:int}/toggle")]
        [AllowAnonymous]
        public ActionResult<LedStateDto> Toggle([FromRoute] int id)
            => Ok(new LedStateDto(_store.Toggle(id)));

        // ===== DHT11 =====
        [HttpPost("dht11")]
        [AllowAnonymous]
        public ActionResult<DhtOutDto> PostDht([FromBody] DhtInDto body)
        {
            var r = new DhtReading
            {
                Utc = DateTime.UtcNow,
                TemperatureC = body.temperatureC,
                Humidity = body.humidity,
                Source = string.IsNullOrWhiteSpace(body.source) ? "esp32" : body.source
            };
            _store.AddDht(r);
            return Ok(new DhtOutDto(r.Utc, r.TemperatureC, r.Humidity, r.Source));
        }

        [HttpGet("dht11/latest")]
        [AllowAnonymous]
        public ActionResult<DhtOutDto> GetLatestDht()
        {
            var r = _store.GetLastDht();
            if (r is null) return NotFound();
            return Ok(new DhtOutDto(r.Utc, r.TemperatureC, r.Humidity, r.Source));
        }

        [HttpGet("dht11/history")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<DhtOutDto>> GetHistory([FromQuery] int take = 50)
        {
            var list = _store.GetDhtHistory(take)
                             .Select(r => new DhtOutDto(r.Utc, r.TemperatureC, r.Humidity, r.Source))
                             .ToList();
            return Ok(list);
        }

        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping() => Ok(new { ok = true, now = DateTime.UtcNow });
    }
}
