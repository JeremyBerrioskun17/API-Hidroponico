using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.Models
{
    [Table("CosechasHidroponico", Schema = "dbo")]
    public class CosechaHidroponico
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long HidroponicoId { get; set; }

        [ForeignKey(nameof(HidroponicoId))]
        public Hidroponico Hidroponico { get; set; } = null!;

        [MaxLength(200)]
        public string? NombreZafra { get; set; } // ej "Zafra 2025-10-01 lote A"

        // NOT NULL en DB: FechaInicio, FechaEstimulada, FechaFin
        public DateTime FechaInicio { get; set; }

        public DateTime FechaEstimulada { get; set; }

        public DateTime FechaFin { get; set; }

        public string? Observaciones { get; set; }

        [Required, MaxLength(50)]
        public string Estado { get; set; } = "ACTIVA"; // ACTIVA, FINALIZADA, PAUSADA

        public DateTime CreadoEn { get; set; }

        // Navegación
        public ICollection<CosechaEtapa>? Etapas { get; set; }
        public ICollection<RiegoHistorico>? Riegos { get; set; }
    }
}
