using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    public class HidroponicoService : IHidroponicoService
    {
        private readonly AppDbContext _db;
        public HidroponicoService(AppDbContext db) { _db = db; }

        public async Task<PagedResult<HidroponicoDto>> GetAsync(int page, int pageSize, string? q, bool includeCosechas, CancellationToken ct = default)
        {
            var query = _db.Hidroponicos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.Trim().ToLower();
                query = query.Where(h =>
                    h.Nombre.ToLower().Contains(ql) ||
                    (h.Observaciones != null && h.Observaciones.ToLower().Contains(ql)) ||
                    h.NumeroHidroponico.ToString().Contains(ql)
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
                        Estado = h.Estado,
                        CreadoEn = h.CreadoEn,
                        Cosechas = h.Cosechas != null ? h.Cosechas.OrderByDescending(c => c.FechaInicio)
                            .Select(c => new CosechaResumenDto
                            {
                                Id = c.Id,
                                NombreZafra = c.NombreZafra,
                                Estado = c.Estado,
                                FechaInicio = c.FechaInicio,
                                FechaFin = c.FechaFin
                            }).ToList() : null
                    }).ToListAsync(ct);

                return new PagedResult<HidroponicoDto> { Page = page, PageSize = pageSize, Total = total, Items = items };
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
                        Estado = h.Estado,
                        CreadoEn = h.CreadoEn
                    }).ToListAsync(ct);

                return new PagedResult<HidroponicoDto> { Page = page, PageSize = pageSize, Total = total, Items = items };
            }
        }

        public async Task<HidroponicoDto?> GetByIdAsync(long id, bool includeCosechas = false, CancellationToken ct = default)
        {
            if (includeCosechas)
            {
                var e = await _db.Hidroponicos
                    .AsNoTracking()
                    .Include(h => h.Cosechas)
                    .FirstOrDefaultAsync(h => h.Id == id, ct);
                if (e is null) return null;
                return new HidroponicoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    NumeroHidroponico = e.NumeroHidroponico,
                    Observaciones = e.Observaciones,
                    CantidadBandejas = e.CantidadBandejas,
                    Estado = e.Estado,
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
                };
            }
            else
            {
                var e = await _db.Hidroponicos.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id, ct);
                if (e is null) return null;
                return new HidroponicoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    NumeroHidroponico = e.NumeroHidroponico,
                    Observaciones = e.Observaciones,
                    CantidadBandejas = e.CantidadBandejas,
                    Estado = e.Estado,
                    CreadoEn = e.CreadoEn
                };
            }
        }

        public async Task<HidroponicoDto> CreateAsync(CreateHidroponicoDto dto, CancellationToken ct = default)
        {
            var e = new Hidroponico
            {
                Nombre = dto.Nombre.Trim(),
                NumeroHidroponico = dto.NumeroHidroponico,
                Observaciones = dto.Observaciones,
                CantidadBandejas = dto.CantidadBandejas,
                Estado = dto.Estado,
                CreadoEn = DateTime.UtcNow
            };

            _db.Hidroponicos.Add(e);
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Hidroponicos_Numero", StringComparison.OrdinalIgnoreCase) == true
                                             || ex.InnerException?.Message.Contains("constraint", StringComparison.OrdinalIgnoreCase) == true)
            {
                throw new InvalidOperationException("Ya existe un Hidropónico con ese NumeroHidroponico.");
            }

            return new HidroponicoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                NumeroHidroponico = e.NumeroHidroponico,
                Observaciones = e.Observaciones,
                CantidadBandejas = e.CantidadBandejas,
                Estado = e.Estado,
                CreadoEn = e.CreadoEn
            };
        }

        public async Task<HidroponicoDto?> UpdateAsync(long id, UpdateHidroponicoDto dto, CancellationToken ct = default)
        {
            var e = await _db.Hidroponicos.FirstOrDefaultAsync(h => h.Id == id, ct);
            if (e is null) return null;

            e.Nombre = dto.Nombre.Trim();
            e.NumeroHidroponico = dto.NumeroHidroponico;
            e.Observaciones = dto.Observaciones;
            e.CantidadBandejas = dto.CantidadBandejas;
            e.Estado = dto.Estado;

            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Hidroponicos_Numero", StringComparison.OrdinalIgnoreCase) == true
                                             || ex.InnerException?.Message.Contains("constraint", StringComparison.OrdinalIgnoreCase) == true)
            {
                throw new InvalidOperationException("Ya existe un Hidropónico con ese NumeroHidroponico.");
            }

            return new HidroponicoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                NumeroHidroponico = e.NumeroHidroponico,
                Observaciones = e.Observaciones,
                CantidadBandejas = e.CantidadBandejas,
                Estado = e.Estado,
                CreadoEn = e.CreadoEn
            };
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
        {
            var e = await _db.Hidroponicos.FirstOrDefaultAsync(h => h.Id == id, ct);
            if (e is null) return false;
            _db.Hidroponicos.Remove(e);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IEnumerable<HidroponicoDto>> ListAllAsync(CancellationToken ct = default)
        {
            var items = await _db.Hidroponicos.AsNoTracking()
                .OrderBy(h => h.NumeroHidroponico)
                .Select(h => new HidroponicoDto
                {
                    Id = h.Id,
                    Nombre = h.Nombre,
                    NumeroHidroponico = h.NumeroHidroponico,
                    Observaciones = h.Observaciones,
                    CantidadBandejas = h.CantidadBandejas,
                    Estado = h.Estado,
                    CreadoEn = h.CreadoEn
                })
                .ToListAsync(ct);
            return items;
        }
    }
}
