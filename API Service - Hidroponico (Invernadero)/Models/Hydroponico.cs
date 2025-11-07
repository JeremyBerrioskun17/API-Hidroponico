using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.Models
{
    [Table("Hidroponicos", Schema = "dbo")]
    public class Hydroponico
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = null!;

        // En DB es BIGINT NOT NULL
        public long NumeroHidroponico { get; set; }

        public string? Observaciones { get; set; }

        public int? CantidadBandejas { get; set; }

        // Default en DB: SYSUTCDATETIME()
        public DateTime CreadoEn { get; set; }

        // Navegación
        public ICollection<CosechaHidroponico>? Cosechas { get; set; }
        public ICollection<Bandeja>? Bandejas { get; set; }
    }

}
