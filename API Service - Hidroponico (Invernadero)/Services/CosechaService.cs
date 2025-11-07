using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    public class CosechaService : ICosechaService
    {
        private readonly AppDbContext _db;
        public CosechaService(AppDbContext db) { _db = db; }

        public async Task<PagedResult<CosechaHidroponicoDto>> GetAsync(int page, int pageSize, long? hidroponicoId = null, string? estado = null,
            DateTime? desde = null, DateTime? hasta = null, CancellationToken ct = default)
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
                }).ToListAsync(ct);

            return new PagedResult<CosechaHidroponicoDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<CosechaHidroponicoDto?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            var e = await _db.CosechasHidroponico.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
            if (e is null) return null;
            return new CosechaHidroponicoDto
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
            };
        }

        public async Task<CosechaHidroponicoDto> CreateAsync(CreateCosechaHidroponicoDto dto, CancellationToken ct = default)
        {
            // validar hidroponico existe
            var hydro = await _db.Hidroponicos.FirstOrDefaultAsync(h => h.Id == dto.HidroponicoId, ct);
            if (hydro is null) throw new ArgumentException("Hidroponico no existe.");

            // validar fechas: FechaInicio <= FechaEstimulada <= FechaFin
            if (dto.FechaInicio > dto.FechaEstimulada) throw new ArgumentException("FechaInicio debe ser <= FechaEstimulada.");
            if (dto.FechaEstimulada > dto.FechaFin) throw new ArgumentException("FechaEstimulada debe ser <= FechaFin.");

            var e = new CosechaHidroponico
            {
                HidroponicoId = dto.HidroponicoId,
                NombreZafra = dto.NombreZafra,
                FechaInicio = dto.FechaInicio.ToUniversalTime(),
                FechaEstimulada = dto.FechaEstimulada.ToUniversalTime(),
                FechaFin = dto.FechaFin.ToUniversalTime(),
                Observaciones = dto.Observaciones,
                Estado = dto.Estado,
                CreadoEn = DateTime.UtcNow
            };

            _db.CosechasHidroponico.Add(e);
            await _db.SaveChangesAsync(ct);

            // Opcional: crear etapas iniciales en CosechaEtapas copiando EtapasHidroponico si así lo deseas.
            // Lo dejamos para la lógica de negocio más adelante.

            return new CosechaHidroponicoDto
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
            };
        }

        public async Task<CosechaHidroponicoDto?> UpdateAsync(long id, UpdateCosechaHidroponicoDto dto, CancellationToken ct = default)
        {
            var e = await _db.CosechasHidroponico.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (e is null) return null;

            // aplicar cambios parciales
            if (dto.HidroponicoId != 0 && dto.HidroponicoId != e.HidroponicoId)
            {
                var hydro = await _db.Hidroponicos.FirstOrDefaultAsync(h => h.Id == dto.HidroponicoId, ct);
                if (hydro is null) throw new ArgumentException("Hidroponico destino no existe.");
                e.HidroponicoId = dto.HidroponicoId;
            }

            if (dto.NombreZafra is not null) e.NombreZafra = dto.NombreZafra;
            if (dto.FechaInicio != default) e.FechaInicio = dto.FechaInicio.ToUniversalTime();
            if (dto.FechaEstimulada != default) e.FechaEstimulada = dto.FechaEstimulada.ToUniversalTime();
            if (dto.FechaFin != default) e.FechaFin = dto.FechaFin.ToUniversalTime();
            if (dto.Observaciones is not null) e.Observaciones = dto.Observaciones;
            if (!string.IsNullOrWhiteSpace(dto.Estado)) e.Estado = dto.Estado;

            // validar orden de fechas
            if (e.FechaInicio > e.FechaEstimulada) throw new ArgumentException("FechaInicio debe ser <= FechaEstimulada.");
            if (e.FechaEstimulada > e.FechaFin) throw new ArgumentException("FechaEstimulada debe ser <= FechaFin.");

            await _db.SaveChangesAsync(ct);

            return new CosechaHidroponicoDto
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
            };
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
        {
            var e = await _db.CosechasHidroponico.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (e is null) return false;
            _db.CosechasHidroponico.Remove(e);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<List<CosechaResumenDto>> GetCosechasResumenByHydroponicoAsync(long hidroponicoId, CancellationToken ct = default)
        {
            return await _db.CosechasHidroponico
                .AsNoTracking()
                .Where(c => c.HidroponicoId == hidroponicoId)
                .OrderByDescending(c => c.FechaInicio)
                .Select(c => new CosechaResumenDto
                {
                    Id = c.Id,
                    NombreZafra = c.NombreZafra,
                    Estado = c.Estado,
                    FechaInicio = c.FechaInicio,
                    FechaFin = c.FechaFin
                }).ToListAsync(ct);
        }
    }
}
