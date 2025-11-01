// DTOs/DiagnosticoDto.cs
using API_Service___Hidroponico__Invernadero_.Models;
using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.DTOs
{
    public class DiagnosticoDto
    {
        public long Id { get; set; }
        public Guid Codigo { get; set; }
        public long? InspectorId { get; set; }
        public string? FotoDiagnostico { get; set; }
        public DateTime FechaMuestreo { get; set; }
        public string? EtapaFenologica { get; set; }
        public TipoDiagnostico Tipo { get; set; }
        public long? PlagaId { get; set; }
        public long? EnfermedadId { get; set; }
        public byte? Severidad { get; set; }
        public string? Notas { get; set; }
    }

    public class CreateDiagnosticoDto
    {
        [Required] public TipoDiagnostico Tipo { get; set; }
        public long? InspectorId { get; set; }
        public string? FotoDiagnostico { get; set; }
        public DateTime? FechaMuestreo { get; set; }
        [MaxLength(50)] public string? EtapaFenologica { get; set; }
        public long? PlagaId { get; set; }
        public long? EnfermedadId { get; set; }
        [Range(0, 5)] public byte? Severidad { get; set; }
        public string? Notas { get; set; }
    }

    public class UpdateDiagnosticoDto
    {
        public long? InspectorId { get; set; }
        public string? FotoDiagnostico { get; set; }
        public DateTime? FechaMuestreo { get; set; }
        [MaxLength(50)] public string? EtapaFenologica { get; set; }
        public TipoDiagnostico? Tipo { get; set; }
        public long? PlagaId { get; set; }
        public long? EnfermedadId { get; set; }
        [Range(0, 5)] public byte? Severidad { get; set; }
        public string? Notas { get; set; }
    }

    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }
}
