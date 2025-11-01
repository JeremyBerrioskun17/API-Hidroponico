using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Service___Hidroponico__Invernadero_.Models
{
    public class Plaga
    {
        [Key] public long Id { get; set; }

        [Required, MaxLength(150)]
        public string NombreComun { get; set; } = null!;

        [MaxLength(150)]
        public string? NombreCientifico { get; set; }

        [MaxLength(100)]
        public string? CicloVida { get; set; }

        public string? Descripcion { get; set; }

        [MaxLength(500)]
        public string? FotoUrl { get; set; }

        [MaxLength(200)]
        public string? PartesAfectadas { get; set; }

        [MaxLength(100)]
        public string? Temporada { get; set; }

        [Range(0, 5)]
        public byte NivelRiesgo { get; set; } = 0;

        public DateTime CreadoEn { get; set; }  // default en SQL
    }
}
