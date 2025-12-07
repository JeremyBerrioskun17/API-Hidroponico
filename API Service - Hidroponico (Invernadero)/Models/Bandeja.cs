using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.Models
{
    [Table("Bandejas", Schema = "dbo")]
    public class Bandeja
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long HidroponicoId { get; set; }

        [ForeignKey(nameof(HidroponicoId))]
        public Hidroponico Hidroponico { get; set; } = null!;

        public int Numero { get; set; }

        public int? CantidadHoyos { get; set; }

        public string? Observaciones { get; set; }
    }
}
