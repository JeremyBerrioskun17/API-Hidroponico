using API_Service___Hidroponico__Invernadero_.DTOs;

namespace API_Service___Hidroponico__Invernadero_.Services
{
    public interface IHidroponicoService
    {
        Task<PagedResult<HidroponicoDto>> GetAsync(int page, int pageSize, string? q, bool includeCosechas, CancellationToken ct = default);
        Task<HidroponicoDto?> GetByIdAsync(long id, bool includeCosechas = false, CancellationToken ct = default);
        Task<HidroponicoDto> CreateAsync(CreateHidroponicoDto dto, CancellationToken ct = default);
        Task<HidroponicoDto?> UpdateAsync(long id, UpdateHidroponicoDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(long id, CancellationToken ct = default);
        Task<IEnumerable<HidroponicoDto>> ListAllAsync(CancellationToken ct = default); // útil para selects en frontend
    }
}
