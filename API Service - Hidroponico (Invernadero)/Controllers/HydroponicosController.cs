using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Controllers
{
    [ApiController]
    [Route("api/hidroponicos")]
    public class HydroponicosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public HydroponicosController(AppDbContext db) => _db = db;

        // GET api/hidroponicos?q=...&page=1&pageSize=20&includeCosechas=true
        [HttpGet]
        public async Task<ActionResult<object>> Get([FromQuery] string? q = null,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            [FromQuery] bool includeCosechas = false, CancellationToken ct = default)
        {
            var query = _db.Hidroponicos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.Trim().ToLower();
                query = query.Where(h =>
                    h.Nombre.ToLower().Contains(ql) ||
                    (h.Observaciones != null && h.Observaciones.ToLower().Contains(ql))
                );
            }

            var total = await query.LongCountAsync(ct);

            if (includeCosechas)
            {
                var items = await query
                    .OrderBy(h => h.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(h => new HidroponicoDto
                    {
                        Id = h.Id,
                        Nombre = h.Nombre,
                        NumeroHidroponico = h.NumeroHidroponico,
                        Observaciones = h.Observaciones,
                        CantidadBandejas = h.CantidadBandejas,
                        CreadoEn = h.CreadoEn,
                        Cosechas = h.Cosechas != null
                            ? h.Cosechas.OrderByDescending(c => c.FechaInicio)
                                .Select(c => new CosechaResumenDto
                                {
                                    Id = c.Id,
                                    NombreZafra = c.NombreZafra,
                                    Estado = c.Estado,
                                    FechaInicio = c.FechaInicio,
                                    FechaFin = c.FechaFin
                                }).ToList()
                            : new List<CosechaResumenDto>()
                    })
                    .ToListAsync(ct);

                return Ok(new { page, pageSize, total, items });
            }
            else
            {
                var items = await query
                    .OrderBy(h => h.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(h => new HidroponicoDto
                    {
                        Id = h.Id,
                        Nombre = h.Nombre,
                        NumeroHidroponico = h.NumeroHidroponico,
                        Observaciones = h.Observaciones,
                        CantidadBandejas = h.CantidadBandejas,
                        CreadoEn = h.CreadoEn
                    })
                    .ToListAsync(ct);

                return Ok(new { page, pageSize, total, items });
            }
        }

        // GET api/hidroponicos/{id}
        [HttpGet("{id:long}")]
        public async Task<ActionResult<HidroponicoDto>> GetById(long id,
            [FromQuery] bool includeCosechas = false, CancellationToken ct = default)
        {
            if (includeCosechas)
            {
                var e = await _db.Hidroponicos
                    .AsNoTracking()
                    .Include(h => h.Cosechas)
                    .FirstOrDefaultAsync(h => h.Id == id, ct);

                if (e is null) return NotFound();

                return Ok(new HidroponicoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    NumeroHidroponico = e.NumeroHidroponico,
                    Observaciones = e.Observaciones,
                    CantidadBandejas = e.CantidadBandejas,
                    CreadoEn = e.CreadoEn,
                    Cosechas = e.Cosechas?.OrderByDescending(c => c.FechaInicio)
                        .Select(c => new CosechaResumenDto
                        {
                            Id = c.Id,
                            NombreZafra = c.NombreZafra,
                            Estado = c.Estado,
                            FechaInicio = c.FechaInicio,
                            FechaFin = c.FechaFin
                        }).ToList()
                });
            }
            else
            {
                var e = await _db.Hidroponicos.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id, ct);
                if (e is null) return NotFound();
                return Ok(new HidroponicoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    NumeroHidroponico = e.NumeroHidroponico,
                    Observaciones = e.Observaciones,
                    CantidadBandejas = e.CantidadBandejas,
                    CreadoEn = e.CreadoEn
                });
            }
        }

        // GET api/hidroponicos/{id}/cosechas
        [HttpGet("{id:long}/cosechas")]
        public async Task<ActionResult<object>> GetCosechas(long id,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        {
            var exists = await _db.Hidroponicos.AsNoTracking().AnyAsync(h => h.Id == id, ct);
            if (!exists) return NotFound();

            var query = _db.CosechasHidroponico.AsNoTracking().Where(c => c.HidroponicoId == id);

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

        // POST api/hidroponicos
        [HttpPost]
        public async Task<ActionResult<HidroponicoDto>> Create([FromBody] CreateHidroponicoDto dto, CancellationToken ct = default)
        {
            var entity = new Hidroponico
            {
                Nombre = dto.Nombre.Trim(),
                NumeroHidroponico = dto.NumeroHidroponico,
                Observaciones = dto.Observaciones?.Trim(),
                CantidadBandejas = dto.CantidadBandejas,
                CreadoEn = DateTime.UtcNow
            };

            _db.Hidroponicos.Add(entity);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Hidroponicos_Numero", StringComparison.OrdinalIgnoreCase) == true
                                             || ex.InnerException?.Message.Contains("constraint", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Conflict(new { error = "Ya existe un Hidropónico con ese NumeroHidroponico." });
            }

            var dtoOut = new HidroponicoDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                NumeroHidroponico = entity.NumeroHidroponico,
                Observaciones = entity.Observaciones,
                CantidadBandejas = entity.CantidadBandejas,
                CreadoEn = entity.CreadoEn
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dtoOut);
        }

        // PUT api/hidroponicos/{id}
        [HttpPut("{id:long}")]
        public async Task<ActionResult<HidroponicoDto>> Update(long id, [FromBody] UpdateHidroponicoDto dto, CancellationToken ct = default)
        {
            var e = await _db.Hidroponicos.FirstOrDefaultAsync(h => h.Id == id, ct);
            if (e is null) return NotFound();

            e.Nombre = dto.Nombre.Trim();
            e.NumeroHidroponico = dto.NumeroHidroponico;
            e.Observaciones = dto.Observaciones?.Trim();
            e.CantidadBandejas = dto.CantidadBandejas;

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Hidroponicos_Numero", StringComparison.OrdinalIgnoreCase) == true
                                             || ex.InnerException?.Message.Contains("constraint", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Conflict(new { error = "Ya existe un Hidropónico con ese NumeroHidroponico." });
            }

            return Ok(new HidroponicoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                NumeroHidroponico = e.NumeroHidroponico,
                Observaciones = e.Observaciones,
                CantidadBandejas = e.CantidadBandejas,
                CreadoEn = e.CreadoEn
            });
        }

        // DELETE api/hidroponicos/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            var e = await _db.Hidroponicos.FirstOrDefaultAsync(h => h.Id == id, ct);
            if (e is null) return NotFound();

            _db.Hidroponicos.Remove(e);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
