// Services/IDiagnosticoService.cs
using API_Service___Hidroponico__Invernadero_.DTOs;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    public interface IDiagnosticoService
    {
        Task<PagedResult<DiagnosticoDto>> GetAsync(
            int page, int pageSize, string? tipo, long? inspectorId,
            DateTime? desdeUtc, DateTime? hastaUtc, long? plagaId, long? enfermedadId,
            CancellationToken ct = default);

        Task<DiagnosticoDto?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<DiagnosticoDto> CreateAsync(CreateDiagnosticoDto dto, CancellationToken ct = default);
        Task<DiagnosticoDto?> UpdateAsync(long id, UpdateDiagnosticoDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    }
}
