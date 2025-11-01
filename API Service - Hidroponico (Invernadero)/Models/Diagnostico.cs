// Models/Diagnostico.cs
using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.Models
{
    public enum TipoDiagnostico
    {
        plaga = 0,
        enfermedad = 1
    }

    public class Diagnostico
    {
        [Key] public long Id { get; set; }
        public Guid Codigo { get; set; }                 // NEWSEQUENTIALID() en SQL
        public long? InspectorId { get; set; }
        public string? FotoDiagnostico { get; set; }
        public DateTime FechaMuestreo { get; set; }      // UTC
        [MaxLength(50)] public string? EtapaFenologica { get; set; }
        [Required] public TipoDiagnostico Tipo { get; set; }
        public long? PlagaId { get; set; }
        public long? EnfermedadId { get; set; }
        [Range(0, 5)] public byte? Severidad { get; set; }
        public string? Notas { get; set; }
    }
}
