using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.Models
{
    [Table("EtapasHidroponico", Schema = "dbo")]
    public class EtapaHidroponico
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Codigo { get; set; } = null!; // 'INICIO','CARPA','LUZ','DECISION'

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = null!;

        public int OrdenEtapa { get; set; }

        public int DuracionHoras { get; set; } // duración estimada en horas

        public bool RequiereLavado { get; set; } = false;

        public string? Observaciones { get; set; }

        // Navegación inversa
        public ICollection<CosechaEtapa>? CosechaEtapas { get; set; }
    }
}
