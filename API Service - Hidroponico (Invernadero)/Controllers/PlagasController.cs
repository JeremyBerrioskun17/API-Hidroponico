using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Controllers
{
    [ApiController]
    [Route("api/plagas")]
    public class PlagasController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PlagasController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<object>> Get([FromQuery] string? q = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var query = _db.Plagas.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.Trim().ToLower();
                query = query.Where(p =>
                    p.NombreComun.ToLower().Contains(ql) ||
                    (p.NombreCientifico != null && p.NombreCientifico.ToLower().Contains(ql)) ||
                    (p.PartesAfectadas != null && p.PartesAfectadas.ToLower().Contains(ql)) ||
                    (p.Temporada != null && p.Temporada.ToLower().Contains(ql))
                );
            }

            var total = await query.LongCountAsync(ct);
            var items = await query
                .OrderBy(p => p.NombreComun)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PlagaDto
                {
                    Id = p.Id,
                    NombreComun = p.NombreComun,
                    NombreCientifico = p.NombreCientifico,
                    CicloVida = p.CicloVida,
                    Descripcion = p.Descripcion,
                    FotoUrl = p.FotoUrl,
                    PartesAfectadas = p.PartesAfectadas,
                    Temporada = p.Temporada,
                    NivelRiesgo = p.NivelRiesgo,
                    CreadoEn = p.CreadoEn
                })
                .ToListAsync(ct);

            return Ok(new { page, pageSize, total, items });
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<PlagaDto>> GetById(long id, CancellationToken ct = default)
        {
            var p = await _db.Plagas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            return p is null ? NotFound() : Ok(new PlagaDto
            {
                Id = p.Id,
                NombreComun = p.NombreComun,
                NombreCientifico = p.NombreCientifico,
                CicloVida = p.CicloVida,
                Descripcion = p.Descripcion,
                FotoUrl = p.FotoUrl,
                PartesAfectadas = p.PartesAfectadas,
                Temporada = p.Temporada,
                NivelRiesgo = p.NivelRiesgo,
                CreadoEn = p.CreadoEn
            });
        }

        [HttpPost]
        public async Task<ActionResult<PlagaDto>> Create([FromBody] CreatePlagaDto dto, CancellationToken ct = default)
        {
            if (dto.NivelRiesgo is < 0 or > 5) return BadRequest("NivelRiesgo debe estar entre 0 y 5.");

            var entity = new Plaga
            {
                NombreComun = dto.NombreComun.Trim(),
                NombreCientifico = dto.NombreCientifico?.Trim(),
                CicloVida = dto.CicloVida?.Trim(),
                Descripcion = dto.Descripcion,
                FotoUrl = dto.FotoUrl?.Trim(),
                PartesAfectadas = dto.PartesAfectadas?.Trim(),
                Temporada = dto.Temporada?.Trim(),
                NivelRiesgo = dto.NivelRiesgo
            };

            _db.Plagas.Add(entity);
            try { await _db.SaveChangesAsync(ct); }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_Plagas_NombreComun", StringComparison.OrdinalIgnoreCase) == true)
            { return Conflict(new { error = "Ya existe una plaga con ese NombreComun." }); }

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new PlagaDto
            {
                Id = entity.Id,
                NombreComun = entity.NombreComun,
                NombreCientifico = entity.NombreCientifico,
                CicloVida = entity.CicloVida,
                Descripcion = entity.Descripcion,
                FotoUrl = entity.FotoUrl,
                PartesAfectadas = entity.PartesAfectadas,
                Temporada = entity.Temporada,
                NivelRiesgo = entity.NivelRiesgo,
                CreadoEn = entity.CreadoEn
            });
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<PlagaDto>> Update(long id, [FromBody] UpdatePlagaDto dto, CancellationToken ct = default)
        {
            if (dto.NivelRiesgo is < 0 or > 5) return BadRequest("NivelRiesgo debe estar entre 0 y 5.");

            var p = await _db.Plagas.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (p is null) return NotFound();

            p.NombreComun = dto.NombreComun.Trim();
            p.NombreCientifico = dto.NombreCientifico?.Trim();
            p.CicloVida = dto.CicloVida?.Trim();
            p.Descripcion = dto.Descripcion;
            p.FotoUrl = dto.FotoUrl?.Trim();
            p.PartesAfectadas = dto.PartesAfectadas?.Trim();
            p.Temporada = dto.Temporada?.Trim();
            p.NivelRiesgo = dto.NivelRiesgo;

            try { await _db.SaveChangesAsync(ct); }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_Plagas_NombreComun", StringComparison.OrdinalIgnoreCase) == true)
            { return Conflict(new { error = "Ya existe una plaga con ese NombreComun." }); }

            return Ok(new PlagaDto
            {
                Id = p.Id,
                NombreComun = p.NombreComun,
                NombreCientifico = p.NombreCientifico,
                CicloVida = p.CicloVida,
                Descripcion = p.Descripcion,
                FotoUrl = p.FotoUrl,
                PartesAfectadas = p.PartesAfectadas,
                Temporada = p.Temporada,
                NivelRiesgo = p.NivelRiesgo,
                CreadoEn = p.CreadoEn
            });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            var p = await _db.Plagas.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (p is null) return NotFound();

            _db.Plagas.Remove(p);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
