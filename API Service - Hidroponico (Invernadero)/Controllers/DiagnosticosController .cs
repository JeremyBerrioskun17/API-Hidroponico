using API_Service___Hidroponico__Invernadero_.DTOs;
using API_Service___Hidroponico__Invernadero_.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_Service___Hidroponico__Invernadero_.Controllers;

[ApiController]
[Route("api/[controller]")] // <-- esto genera /api/diagnosticos
public class DiagnosticosController : ControllerBase
{
    private readonly IDiagnosticoService _service;

    public DiagnosticosController(IDiagnosticoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Listado con filtros y paginación
    /// Ejemplo: GET /api/diagnosticos?tipo=plaga&page=1&pageSize=20
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<DiagnosticoDto>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? tipo = null,
        [FromQuery] long? inspectorId = null,
        [FromQuery] DateTime? desdeUtc = null,
        [FromQuery] DateTime? hastaUtc = null,
        [FromQuery] long? plagaId = null,
        [FromQuery] long? enfermedadId = null,
        CancellationToken ct = default)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest("page y pageSize deben ser > 0.");

        var result = await _service.GetAsync(page, pageSize, tipo, inspectorId, desdeUtc, hastaUtc, plagaId, enfermedadId, ct);

        // Header para paginación (opcional)
        Response.Headers["X-Total-Count"] = result.Total.ToString();

        return Ok(result);
    }

    /// <summary>
    /// Obtiene un diagnóstico por ID
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<DiagnosticoDto>> GetById(long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Crea un nuevo diagnóstico
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DiagnosticoDto>> Create([FromBody] CreateDiagnosticoDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un diagnóstico
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<DiagnosticoDto>> Update(long id, [FromBody] UpdateDiagnosticoDto dto, CancellationToken ct)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un diagnóstico
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
