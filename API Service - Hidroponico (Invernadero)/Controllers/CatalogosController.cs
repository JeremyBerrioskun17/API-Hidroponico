using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Controllers
{
    [ApiController]
    [Route("api/catalogos")]
    public class CatalogosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CatalogosController(AppDbContext db) => _db = db;

        [HttpGet("plagas")]
        public async Task<ActionResult<IEnumerable<CatalogItemDto>>> GetPlagas([FromQuery] string? q = null, [FromQuery] int top = 200, CancellationToken ct = default)
        {
            var query = _db.Plagas.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.Trim().ToLower();
                query = query.Where(p =>
                    p.NombreComun.ToLower().Contains(ql) ||
                    (p.NombreCientifico != null && p.NombreCientifico.ToLower().Contains(ql))
                );
            }

            var items = await query
                .OrderBy(p => p.NombreComun)
                .Take(Math.Clamp(top, 1, 1000))
                .Select(p => new CatalogItemDto
                {
                    Id = p.Id,
                    Nombre = p.NombreComun,           // <- devolvemos NombreComun como "Nombre"
                    Extra = p.NombreCientifico        // <- opcional
                })
                .ToListAsync(ct);

            return Ok(items);
        }

        [HttpGet("enfermedades")]
        public async Task<ActionResult<IEnumerable<CatalogItemDto>>> GetEnfermedades([FromQuery] string? q = null, [FromQuery] int top = 200, CancellationToken ct = default)
        {
            var query = _db.Enfermedades.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.Trim().ToLower();
                query = query.Where(e =>
                    e.NombreComun.ToLower().Contains(ql) ||
                    (e.AgenteCausal != null && e.AgenteCausal.ToLower().Contains(ql))
                );
            }

            var items = await query
                .OrderBy(e => e.NombreComun)
                .Take(Math.Clamp(top, 1, 1000))
                .Select(e => new CatalogItemDto
                {
                    Id = e.Id,
                    Nombre = e.NombreComun,           // <- devolvemos NombreComun como "Nombre"
                    Extra = e.AgenteCausal            // <- opcional
                })
                .ToListAsync(ct);

            return Ok(items);
        }
    }
}