using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.DTOs
{
    public class EnfermedadDto
    {
        public long Id { get; set; }
        public string NombreComun { get; set; } = null!;
        public string? AgenteCausal { get; set; }
        public string? TipoPatogeno { get; set; }
        public string? Descripcion { get; set; }
        public string? FotoUrl { get; set; }
        public string? PartesAfectadas { get; set; }
        public string? Temporada { get; set; }
        public byte NivelRiesgo { get; set; }
        public DateTime CreadoEn { get; set; }
    }

    public class CreateEnfermedadDto
    {
        [Required, MaxLength(150)]
        public string NombreComun { get; set; } = null!;

        [MaxLength(150)]
        public string? AgenteCausal { get; set; }

        [MaxLength(20)]
        public string? TipoPatogeno { get; set; } // validar en controller

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

    public class UpdateEnfermedadDto : CreateEnfermedadDto { }
}