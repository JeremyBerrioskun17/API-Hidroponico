using API_Service___Hidroponico__Invernadero_.DTOs;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    public interface IHydroponicoService
    {
        Task<PagedResult<HydroponicoDto>> GetAsync(int page, int pageSize, bool includeCosechas = false, CancellationToken ct = default);
        Task<HydroponicoDto?> GetByIdAsync(long id, bool includeCosechas = false, CancellationToken ct = default);
        Task<HydroponicoDto> CreateAsync(CreateHydroponicoDto dto, CancellationToken ct = default);
        Task<HydroponicoDto?> UpdateAsync(long id, UpdateHydroponicoDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    }
}
