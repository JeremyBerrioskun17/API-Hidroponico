using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.Models
{
    [Table("Hidroponicos", Schema = "dbo")]
    public class Hidroponico
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = null!;

        [Required]
        public long NumeroHidroponico { get; set; }

        // Observaciones en NVARCHAR(MAX)
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? Observaciones { get; set; }

        public int? CantidadBandejas { get; set; }

        // 0 = libre/desocupado, 1 = ocupado, etc. según tu lógica
        public int Estado { get; set; } = 0;

        public DateTime CreadoEn { get; set; } // default en SQL

        // Navegación: cosechas asociadas (opcional)
        public ICollection<CosechaHidroponico>? Cosechas { get; set; }

        // Navegación: bandejas
        public ICollection<Bandeja>? Bandejas { get; set; }
    }
}
