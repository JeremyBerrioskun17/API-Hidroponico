using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Controllers
{
    [ApiController]
    [Route("api/enfermedades")]
    public class EnfermedadesController : ControllerBase
    {
        private static readonly HashSet<string> _tiposPatogeno = new(StringComparer.OrdinalIgnoreCase)
    { "hongo","bacteria","virus","nematodo","oomiceto","fitoplasma","otro" };

        private readonly AppDbContext _db;
        public EnfermedadesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<object>> Get([FromQuery] string? q = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var query = _db.Enfermedades.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.Trim().ToLower();
                query = query.Where(e =>
                    e.NombreComun.ToLower().Contains(ql) ||
                    (e.AgenteCausal != null && e.AgenteCausal.ToLower().Contains(ql)) ||
                    (e.TipoPatogeno != null && e.TipoPatogeno.ToLower().Contains(ql)) ||
                    (e.PartesAfectadas != null && e.PartesAfectadas.ToLower().Contains(ql)) ||
                    (e.Temporada != null && e.Temporada.ToLower().Contains(ql))
                );
            }

            var total = await query.LongCountAsync(ct);
            var items = await query
                .OrderBy(e => e.NombreComun)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EnfermedadDto
                {
                    Id = e.Id,
                    NombreComun = e.NombreComun,
                    AgenteCausal = e.AgenteCausal,
                    TipoPatogeno = e.TipoPatogeno,
                    Descripcion = e.Descripcion,
                    FotoUrl = e.FotoUrl,
                    PartesAfectadas = e.PartesAfectadas,
                    Temporada = e.Temporada,
                    NivelRiesgo = e.NivelRiesgo,
                    CreadoEn = e.CreadoEn
                })
                .ToListAsync(ct);

            return Ok(new { page, pageSize, total, items });
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<EnfermedadDto>> GetById(long id, CancellationToken ct = default)
        {
            var e = await _db.Enfermedades.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            return e is null ? NotFound() : Ok(new EnfermedadDto
            {
                Id = e.Id,
                NombreComun = e.NombreComun,
                AgenteCausal = e.AgenteCausal,
                TipoPatogeno = e.TipoPatogeno,
                Descripcion = e.Descripcion,
                FotoUrl = e.FotoUrl,
                PartesAfectadas = e.PartesAfectadas,
                Temporada = e.Temporada,
                NivelRiesgo = e.NivelRiesgo,
                CreadoEn = e.CreadoEn
            });
        }

        [HttpPost]
        public async Task<ActionResult<EnfermedadDto>> Create([FromBody] CreateEnfermedadDto dto, CancellationToken ct = default)
        {
            if (dto.NivelRiesgo is < 0 or > 5) return BadRequest("NivelRiesgo debe estar entre 0 y 5.");
            if (!string.IsNullOrWhiteSpace(dto.TipoPatogeno) && !_tiposPatogeno.Contains(dto.TipoPatogeno))
                return BadRequest("TipoPatogeno inválido.");

            var entity = new Enfermedad
            {
                NombreComun = dto.NombreComun.Trim(),
                AgenteCausal = dto.AgenteCausal?.Trim(),
                TipoPatogeno = dto.TipoPatogeno?.Trim(),
                Descripcion = dto.Descripcion,
                FotoUrl = dto.FotoUrl?.Trim(),
                PartesAfectadas = dto.PartesAfectadas?.Trim(),
                Temporada = dto.Temporada?.Trim(),
                NivelRiesgo = dto.NivelRiesgo
            };

            _db.Enfermedades.Add(entity);
            try { await _db.SaveChangesAsync(ct); }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_Enfermedades_NombreComun", StringComparison.OrdinalIgnoreCase) == true)
            { return Conflict(new { error = "Ya existe una enfermedad con ese NombreComun." }); }

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new EnfermedadDto
            {
                Id = entity.Id,
                NombreComun = entity.NombreComun,
                AgenteCausal = entity.AgenteCausal,
                TipoPatogeno = entity.TipoPatogeno,
                Descripcion = entity.Descripcion,
                FotoUrl = entity.FotoUrl,
                PartesAfectadas = entity.PartesAfectadas,
                Temporada = entity.Temporada,
                NivelRiesgo = entity.NivelRiesgo,
                CreadoEn = entity.CreadoEn
            });
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<EnfermedadDto>> Update(long id, [FromBody] UpdateEnfermedadDto dto, CancellationToken ct = default)
        {
            if (dto.NivelRiesgo is < 0 or > 5) return BadRequest("NivelRiesgo debe estar entre 0 y 5.");
            if (!string.IsNullOrWhiteSpace(dto.TipoPatogeno) && !_tiposPatogeno.Contains(dto.TipoPatogeno))
                return BadRequest("TipoPatogeno inválido.");

            var e = await _db.Enfermedades.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return NotFound();

            e.NombreComun = dto.NombreComun.Trim();
            e.AgenteCausal = dto.AgenteCausal?.Trim();
            e.TipoPatogeno = dto.TipoPatogeno?.Trim();
            e.Descripcion = dto.Descripcion;
            e.FotoUrl = dto.FotoUrl?.Trim();
            e.PartesAfectadas = dto.PartesAfectadas?.Trim();
            e.Temporada = dto.Temporada?.Trim();
            e.NivelRiesgo = dto.NivelRiesgo;

            try { await _db.SaveChangesAsync(ct); }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_Enfermedades_NombreComun", StringComparison.OrdinalIgnoreCase) == true)
            { return Conflict(new { error = "Ya existe una enfermedad con ese NombreComun." }); }

            return Ok(new EnfermedadDto
            {
                Id = e.Id,
                NombreComun = e.NombreComun,
                AgenteCausal = e.AgenteCausal,
                TipoPatogeno = e.TipoPatogeno,
                Descripcion = e.Descripcion,
                FotoUrl = e.FotoUrl,
                PartesAfectadas = e.PartesAfectadas,
                Temporada = e.Temporada,
                NivelRiesgo = e.NivelRiesgo,
                CreadoEn = e.CreadoEn
            });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
        {
            var e = await _db.Enfermedades.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return NotFound();

            _db.Enfermedades.Remove(e);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}