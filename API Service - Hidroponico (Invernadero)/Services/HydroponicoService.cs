using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    public class HydroponicoService : IHydroponicoService
    {
        private readonly AppDbContext _db;
        public HydroponicoService(AppDbContext db) { _db = db; }

        public async Task<PagedResult<HydroponicoDto>> GetAsync(int page, int pageSize, bool includeCosechas = false, CancellationToken ct = default)
        {
            var query = _db.Hidroponicos.AsNoTracking().AsQueryable();

            var total = await query.LongCountAsync(ct);

            if (includeCosechas)
            {
                // traer anidado
                var items = await query
                    .OrderBy(h => h.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(h => new HydroponicoDto
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
                    }).ToListAsync(ct);

                return new PagedResult<HydroponicoDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    Items = items
                };
            }
            else
            {
                var items = await query
                    .OrderBy(h => h.Nombre)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(h => new HydroponicoDto
                    {
                        Id = h.Id,
                        Nombre = h.Nombre,
                        NumeroHidroponico = h.NumeroHidroponico,
                        Observaciones = h.Observaciones,
                        CantidadBandejas = h.CantidadBandejas,
                        CreadoEn = h.CreadoEn
                    }).ToListAsync(ct);

                return new PagedResult<HydroponicoDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    Items = items
                };
            }
        }

        public async Task<HydroponicoDto?> GetByIdAsync(long id, bool includeCosechas = false, CancellationToken ct = default)
        {
            if (includeCosechas)
            {
                var e = await _db.Hidroponicos
                    .AsNoTracking()
                    .Include(h => h.Cosechas)
                    .FirstOrDefaultAsync(h => h.Id == id, ct);
                if (e is null) return null;

                return new HydroponicoDto
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
                };
            }
            else
            {
                var e = await _db.Hidroponicos.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id, ct);
                if (e is null) return null;
                return new HydroponicoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    NumeroHidroponico = e.NumeroHidroponico,
                    Observaciones = e.Observaciones,
                    CantidadBandejas = e.CantidadBandejas,
                    CreadoEn = e.CreadoEn
                };
            }
        }

        public async Task<HydroponicoDto> CreateAsync(CreateHydroponicoDto dto, CancellationToken ct = default)
        {
            // opcional: validar número único si se requiere
            var exists = await _db.Hidroponicos.AnyAsync(h => h.NumeroHidroponico == dto.NumeroHidroponico, ct);
            if (exists) throw new ArgumentException("Ya existe un hidroponico con ese NumeroHidroponico.");

            var e = new Hydroponico
            {
                Nombre = dto.Nombre,
                NumeroHidroponico = dto.NumeroHidroponico,
                Observaciones = dto.Observaciones,
                CantidadBandejas = dto.CantidadBandejas,
                CreadoEn = DateTime.UtcNow
            };
            _db.Hidroponicos.Add(e);
            await _db.SaveChangesAsync(ct);

            return new HydroponicoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                NumeroHidroponico = e.NumeroHidroponico,
                Observaciones = e.Observaciones,
                CantidadBandejas = e.CantidadBandejas,
                CreadoEn = e.CreadoEn
            };
        }

        public async Task<HydroponicoDto?> UpdateAsync(long id, UpdateHydroponicoDto dto, CancellationToken ct = default)
        {
            var e = await _db.Hidroponicos.FirstOrDefaultAsync(h => h.Id == id, ct);
            if (e is null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Nombre)) e.Nombre = dto.Nombre;
            if (dto.NumeroHidroponico != 0) e.NumeroHidroponico = dto.NumeroHidroponico;
            if (dto.Observaciones is not null) e.Observaciones = dto.Observaciones;
            if (dto.CantidadBandejas.HasValue) e.CantidadBandejas = dto.CantidadBandejas;

            await _db.SaveChangesAsync(ct);

            return new HydroponicoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                NumeroHidroponico = e.NumeroHidroponico,
                Observaciones = e.Observaciones,
                CantidadBandejas = e.CantidadBandejas,
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
    }
}
