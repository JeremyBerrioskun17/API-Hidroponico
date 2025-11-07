using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Controllers
{
    [ApiController]
    [Route("api/cosechas")]
    public class CosechasController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CosechasController(AppDbContext db) => _db = db;

        // GET api/cosechas?page=1&pageSize=20&hidroponicoId=1&estado=ACTIVA
        [HttpGet]
        public async Task<ActionResult<object>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            [FromQuery] long? hidroponicoId = null, [FromQuery] string? estado = null,
            [FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null,
            CancellationToken ct = default)
        {
            var query = _db.CosechasHidroponico.AsNoTracking().AsQueryable();

            if (hidroponicoId.HasValue) query = query.Where(c => c.HidroponicoId == hidroponicoId.Value);
            if (!string.IsNullOrWhiteSpace(estado)) query = query.Where(c => c.Estado == estado);
            if (desde.HasValue) query = query.Where(c => c.FechaInicio >= desde.Value);
            if (hasta.HasValue) query = query.Where(c => c.FechaInicio < hasta.Value);

            var total = await query.LongCountAsync(ct);

            var items = await query
                .OrderByDescending(c => c.FechaInicio)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CosechaHidroponicoDto
                {
                    Id = c.Id,
                    HidroponicoId = c.HidroponicoId,
                    NombreZafra = c.NombreZafra,
                    FechaInicio = c.FechaInicio,
                    FechaEstimulada = c.FechaEstimulada,
                    FechaFin = c.FechaFin,
                    Observaciones = c.Observaciones,
                    Estado = c.Estado,
                    CreadoEn = c.CreadoEn
                })
                .ToListAsync(ct);

            return Ok(new { page, pageSize, total, items });
        }

        // GET api/cosechas/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<CosechaHidroponicoDto>> GetById(long id, CancellationToken ct = default)
        {
            var e = await _db.CosechasHidroponico.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
            if (e is null) return NotFound();

            return Ok(new CosechaHidroponicoDto
            {
                Id = e.Id,
                HidroponicoId = e.HidroponicoId,
                NombreZafra = e.NombreZafra,
                FechaInicio = e.FechaInicio,
                FechaEstimulada = e.FechaEstimulada,
                FechaFin = e.FechaFin,
                Observaciones = e.Observaciones,
                Estado = e.Estado,
                CreadoEn = e.CreadoEn
            });
        }

        // POST api/cosechas
        [HttpPost]
        public async Task<ActionResult<CosechaHidroponicoDto>> Create([FromBody] CreateCosechaHidroponicoDto dto, CancellationToken ct = default)
        {
            // validar hidroponico existe
            var hydro = await _db.Hidroponicos.AsNoTracking().FirstOrDefaultAsync(h => h.Id == dto.HidroponicoId, ct);
            if (hydro is null) return BadRequest("Hidroponico no existe.");

            // validar orden de fechas
            if (dto.FechaInicio > dto.FechaEstimulada) return BadRequest("FechaInicio debe ser <= FechaEstimulada.");
            if (dto.FechaEstimulada > dto.FechaFin) return BadRequest("FechaEstimulada debe ser <= FechaFin.");

            var e = new CosechaHidroponico
            {
                HidroponicoId = dto.HidroponicoId,
                NombreZafra = dto.NombreZafra?.Trim(),
                FechaInicio = dto.FechaInicio.ToUniversalTime(),
                FechaEstimulada = dto.FechaEstimulada.ToUniversalTime(),
                FechaFin = dto.FechaFin.ToUniversalTime(),
                Observaciones = dto.Observaciones,
                Estado = dto.Estado,
                CreadoEn = DateTime.UtcNow
            };

            _db.CosechasHidroponico.Add(e);
            await _db.SaveChangesAsync(ct);

            // Nota: si quieres crear CosechaEtapas automáticamente aquí lo implementamos

            return CreatedAtAction(nameof(GetById), new { id = e.Id }, new CosechaHidroponicoDto
            {
                Id = e.Id,
                HidroponicoId = e.HidroponicoId,
                NombreZafra = e.NombreZafra,
                FechaInicio = e.FechaInicio,
                FechaEstimulada = e.FechaEstimulada,
                FechaFin = e.FechaFin,
                Observaciones = e.Observaciones,
                Estado = e.Estado,
                CreadoEn = e.CreadoEn
            });
        }

        // PUT api/cosechas/{id}
        [HttpPut("{id:long}")]
        public async Task<ActionResult<CosechaHidroponicoDto>> Update(long id, [FromBody] UpdateCosechaHidroponicoDto dto, CancellationToken ct = default)
        {
            var e = await _db.CosechasHidroponico.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (e is null) return NotFound();

            if (dto.HidroponicoId != 0 && dto.HidroponicoId != e.HidroponicoId)
            {
                var hydro = await _db.Hidroponicos.AsNoTracking().FirstOrDefaultAsync(h => h.Id == dto.HidroponicoId, ct);
                if (hydro is null) return BadRequest("Hidroponico destino no existe.");
                e.HidroponicoId = dto.HidroponicoId;
            }

            e.NombreZafra = dto.NombreZafra?.Trim();
            if (dto.FechaInicio != default) e.FechaInicio = dto.FechaInicio.ToUniversalTime();
            if (dto.FechaEstimulada != default) e.FechaEstimulada = dto.FechaEstimulada.ToUniversalTime();
            if (dto.FechaFin != default) e.FechaFin = dto.FechaFin.ToUniversalTime();
            e.Observaciones = dto.Observaciones;
            e.Estado = dto.Estado;

            // validar orden de fechas
            if (e.FechaInicio > e.FechaEstimulada) return BadRequest("FechaInicio debe ser <= FechaEstimulada.");
            if (e.FechaEstimulada > e.FechaFin) return BadRequest("FechaEstimulada debe ser <= FechaFin.");

            await _db.SaveChangesAsync(ct);

            return Ok(new CosechaHidroponicoDto
            {
                Id = e.Id,
                HidroponicoId = e.HidroponicoId,
                NombreZafra = e.NombreZafra,
                FechaInicio = e.FechaInicio,
                FechaEstimulada = e.FechaEstimulada,
                FechaFin = e.FechaFin,
                Observaciones = e.Observaciones,
                Estado = e.Estado,
                CreadoEn = e.CreadoEn
            });
        }

        // DELETE api/cosechas/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            var e = await _db.CosechasHidroponico.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (e is null) return NotFound();

            _db.CosechasHidroponico.Remove(e);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
