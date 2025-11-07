using API_Service___Hidroponico__Invernadero_.DTOs;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    public interface ICosechaService
    {
        Task<PagedResult<CosechaHidroponicoDto>> GetAsync(int page, int pageSize, long? hidroponicoId = null, string? estado = null,
            DateTime? desde = null, DateTime? hasta = null, CancellationToken ct = default);

        Task<CosechaHidroponicoDto?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<CosechaHidroponicoDto> CreateAsync(CreateCosechaHidroponicoDto dto, CancellationToken ct = default);
        Task<CosechaHidroponicoDto?> UpdateAsync(long id, UpdateCosechaHidroponicoDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(long id, CancellationToken ct = default);

        // utilitario
        Task<List<CosechaResumenDto>> GetCosechasResumenByHydroponicoAsync(long hidroponicoId, CancellationToken ct = default);
    }
}
