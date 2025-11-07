using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.DTOs
{
    public class CosechaHidroponicoDto
    {
        public long Id { get; set; }
        public long HidroponicoId { get; set; }
        public string? NombreZafra { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaEstimulada { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Observaciones { get; set; }
        public string Estado { get; set; } = "ACTIVA";
        public DateTime CreadoEn { get; set; }
    }

    public class CreateCosechaHidroponicoDto
    {
        [Required]
        public long HidroponicoId { get; set; }

        [MaxLength(200)]
        public string? NombreZafra { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaEstimulada { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public string? Observaciones { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; } = "ACTIVA";
    }

    public class UpdateCosechaHidroponicoDto : CreateCosechaHidroponicoDto { }

    // DTO auxiliar para mostrar lista corta de cosechas
    public class CosechaResumenDto
    {
        public long Id { get; set; }
        public string? NombreZafra { get; set; }
        public string Estado { get; set; } = "ACTIVA";
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
