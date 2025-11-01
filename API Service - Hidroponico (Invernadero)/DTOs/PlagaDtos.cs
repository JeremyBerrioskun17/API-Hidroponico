using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.DTOs
{
    public class PlagaDto
    {
        public long Id { get; set; }
        public string NombreComun { get; set; } = null!;
        public string? NombreCientifico { get; set; }
        public string? CicloVida { get; set; }
        public string? Descripcion { get; set; }
        public string? FotoUrl { get; set; }
        public string? PartesAfectadas { get; set; }
        public string? Temporada { get; set; }
        public byte NivelRiesgo { get; set; }
        public DateTime CreadoEn { get; set; }
    }

    public class CreatePlagaDto
    {
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
    }

    public class UpdatePlagaDto : CreatePlagaDto { }
}