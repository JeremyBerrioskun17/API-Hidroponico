using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.Models
{
    [Table("CosechaEtapas", Schema = "dbo")]
    public class CosechaEtapa
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long CosechaId { get; set; }

        [ForeignKey(nameof(CosechaId))]
        public CosechaHidroponico Cosecha { get; set; } = null!;

        [Required]
        public int EtapaId { get; set; }

        [ForeignKey(nameof(EtapaId))]
        public EtapaHidroponico Etapa { get; set; } = null!;

        // Fecha real de inicio (NOT NULL)
        public DateTime FechaInicioReal { get; set; }

        // Fecha real de fin (NULL si sigue)
        public DateTime? FechaFinReal { get; set; }

        // copia de la duracion plan en horas al crear la etapa
        public int DuracionHorasPlan { get; set; }

        // intervalo de riego en segundos durante la etapa (si aplica)
        public int? RiegoProgramadoIntervaloSegundos { get; set; }

        public string? Notas { get; set; }
    }
}
