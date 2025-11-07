using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.Models
{
    [Table("RiegosHistorico", Schema = "dbo")]
    public class RiegoHistorico
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long CosechaId { get; set; }

        [ForeignKey(nameof(CosechaId))]
        public CosechaHidroponico Cosecha { get; set; } = null!;

        public int? EtapaId { get; set; } // si corresponde a una etapa
        // no agregamos navegación directa a Etapa para evitar FK ambiguos; si lo quieres se puede añadir
        public long? HidroponicoId { get; set; } // redundante para consultas

        public DateTime InicioRiego { get; set; }

        public int? DuracionSeg { get; set; }

        [MaxLength(100)]
        public string? Fuente { get; set; } // 'programado','manual','sensor'

        public string? Observaciones { get; set; }
    }
}
