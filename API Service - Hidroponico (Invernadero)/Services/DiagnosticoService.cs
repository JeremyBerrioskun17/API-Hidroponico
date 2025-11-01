// Services/DiagnosticoService.cs
using API_Service___Hidroponico__Invernadero_.Data;
using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    public class DiagnosticoService : IDiagnosticoService
    {
        private readonly AppDbContext _db;
        public DiagnosticoService(AppDbContext db) { _db = db; }

        public async Task<PagedResult<DiagnosticoDto>> GetAsync(
            int page, int pageSize, string? tipo, long? inspectorId,
            DateTime? desdeUtc, DateTime? hastaUtc, long? plagaId, long? enfermedadId,
            CancellationToken ct = default)
        {
            var query = _db.Diagnosticos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipo) &&
                Enum.TryParse<TipoDiagnostico>(tipo, true, out var tipoEnum))
            {
                query = query.Where(x => x.Tipo == tipoEnum);
            }
            if (inspectorId.HasValue) query = query.Where(x => x.InspectorId == inspectorId);
            if (plagaId.HasValue) query = query.Where(x => x.PlagaId == plagaId);
            if (enfermedadId.HasValue) query = query.Where(x => x.EnfermedadId == enfermedadId);
            if (desdeUtc.HasValue) query = query.Where(x => x.FechaMuestreo >= desdeUtc.Value);
            if (hastaUtc.HasValue) query = query.Where(x => x.FechaMuestreo < hastaUtc.Value);

            var total = await query.LongCountAsync(ct);
            var items = await query
    .OrderByDescending(x => x.FechaMuestreo)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(x => new DiagnosticoDto
    {
        Id = x.Id,
        Codigo = x.Codigo,
        InspectorId = x.InspectorId,
        FotoDiagnostico = x.FotoDiagnostico,
        FechaMuestreo = x.FechaMuestreo, // ya es UTC en DB
        EtapaFenologica = x.EtapaFenologica,
        Tipo = x.Tipo,
        PlagaId = x.PlagaId,
        EnfermedadId = x.EnfermedadId,
        Severidad = x.Severidad,
        Notas = x.Notas
    })
    .ToListAsync(ct);


            return new PagedResult<DiagnosticoDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<DiagnosticoDto?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            var e = await _db.Diagnosticos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            return e is null ? null : MapToDto(e);
        }

        public async Task<DiagnosticoDto> CreateAsync(CreateDiagnosticoDto dto, CancellationToken ct = default)
        {
            ValidateTipoAndForeign(dto.Tipo, dto.PlagaId, dto.EnfermedadId);
            var e = new Diagnostico
            {
                InspectorId = dto.InspectorId,
                FotoDiagnostico = dto.FotoDiagnostico,
                FechaMuestreo = dto.FechaMuestreo?.ToUniversalTime() ?? DateTime.UtcNow,
                EtapaFenologica = dto.EtapaFenologica,
                Tipo = dto.Tipo,
                PlagaId = dto.PlagaId,
                EnfermedadId = dto.EnfermedadId,
                Severidad = dto.Severidad,
                Notas = dto.Notas
            };
            _db.Diagnosticos.Add(e);
            await _db.SaveChangesAsync(ct);
            return MapToDto(e);
        }

        public async Task<DiagnosticoDto?> UpdateAsync(long id, UpdateDiagnosticoDto dto, CancellationToken ct = default)
        {
            var e = await _db.Diagnosticos.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return null;

            var newTipo = dto.Tipo ?? e.Tipo;
            var newPlagaId = dto.PlagaId ?? e.PlagaId;
            var newEnfermedadId = dto.EnfermedadId ?? e.EnfermedadId;
            ValidateTipoAndForeign(newTipo, newPlagaId, newEnfermedadId);

            if (dto.InspectorId.HasValue) e.InspectorId = dto.InspectorId;
            if (dto.FotoDiagnostico is not null) e.FotoDiagnostico = dto.FotoDiagnostico;
            if (dto.FechaMuestreo.HasValue) e.FechaMuestreo = dto.FechaMuestreo.Value.ToUniversalTime();
            if (dto.EtapaFenologica is not null) e.EtapaFenologica = dto.EtapaFenologica;
            if (dto.Tipo.HasValue) e.Tipo = dto.Tipo.Value;
            if (dto.PlagaId.HasValue || dto.Tipo == TipoDiagnostico.plaga) e.PlagaId = dto.PlagaId;
            if (dto.EnfermedadId.HasValue || dto.Tipo == TipoDiagnostico.enfermedad) e.EnfermedadId = dto.EnfermedadId;
            if (dto.Severidad.HasValue) e.Severidad = dto.Severidad;
            if (dto.Notas is not null) e.Notas = dto.Notas;

            await _db.SaveChangesAsync(ct);
            return MapToDto(e);
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
        {
            var e = await _db.Diagnosticos.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return false;
            _db.Diagnosticos.Remove(e);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        private static DiagnosticoDto MapToDto(Diagnostico x) => new()
        {
            Id = x.Id,
            Codigo = x.Codigo,
            InspectorId = x.InspectorId,
            FotoDiagnostico = x.FotoDiagnostico,
            FechaMuestreo = DateTime.SpecifyKind(x.FechaMuestreo, DateTimeKind.Utc),
            EtapaFenologica = x.EtapaFenologica,
            Tipo = x.Tipo,
            PlagaId = x.PlagaId,
            EnfermedadId = x.EnfermedadId,
            Severidad = x.Severidad,
            Notas = x.Notas
        };

        private static void ValidateTipoAndForeign(TipoDiagnostico tipo, long? plagaId, long? enfermedadId)
        {
            if (tipo == TipoDiagnostico.plaga)
            {
                if (!plagaId.HasValue || enfermedadId.HasValue)
                    throw new ArgumentException("Para Tipo=plaga, PlagaId es requerido y EnfermedadId debe ser NULL.");
            }
            else
            {
                if (!enfermedadId.HasValue || plagaId.HasValue)
                    throw new ArgumentException("Para Tipo=enfermedad, EnfermedadId es requerido y PlagaId debe ser NULL.");
            }
        }
    }
}
