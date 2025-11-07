using System.ComponentModel.DataAnnotations;

namespace API_Service___Hidroponico__Invernadero_.DTOs
{
    public class HydroponicoDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = null!;
        public long NumeroHidroponico { get; set; }
        public string? Observaciones { get; set; }
        public int? CantidadBandejas { get; set; }
        public DateTime CreadoEn { get; set; }

        // Lista opcional de cosechas resumidas
        public List<CosechaResumenDto>? Cosechas { get; set; }
    }

    public class CreateHydroponicoDto
    {
        [Required, MaxLength(150)]
        public string Nombre { get; set; } = null!;

        [Required]
        public long NumeroHidroponico { get; set; }

        public string? Observaciones { get; set; }

        public int? CantidadBandejas { get; set; }
    }

    public class UpdateHydroponicoDto : CreateHydroponicoDto { }
}

