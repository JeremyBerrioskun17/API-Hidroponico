namespace API_Service___Hidroponico__Invernadero_.DTOs
{
    public class CatalogItemDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = null!;     // regresamos NombreComun como "Nombre"
        public string? Extra { get; set; }              // NombreCientifico / AgenteCausal (opcional)
    }
}
