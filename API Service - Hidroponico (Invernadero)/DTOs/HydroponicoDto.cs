using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.DTOs
{
    public class HidroponicoDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = null!;
        public long NumeroHidroponico { get; set; }
        public string? Observaciones { get; set; }
        public int? CantidadBandejas { get; set; }
        public int Estado { get; set; }
        public DateTime CreadoEn { get; set; }

        // Opcional: resumen de cosechas si solicitas includeCosechas en controller
        public List<CosechaResumenDto>? Cosechas { get; set; }
    }

    public class CreateHidroponicoDto
    {
        [Required, MaxLength(150)]
        public string Nombre { get; set; } = null!;

        [Required]
        public long NumeroHidroponico { get; set; }

        public string? Observaciones { get; set; }
        public int? CantidadBandejas { get; set; }

        // Opcional: permitir especificar estado al crear, por defecto 0
        public int Estado { get; set; } = 0;
    }

    public class UpdateHidroponicoDto : CreateHidroponicoDto
    {
    }
}

