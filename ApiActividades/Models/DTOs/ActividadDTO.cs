namespace ApiActividades.Models.DTOs
{
    public class ActividadDTO
    {
        public int? Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Imagen { get; set; }
        public DateOnly? FechaRealizacion { get; set; }
        public string NombreDepartamento { get; set; } = string.Empty;
        public int? Estado { get; set; }

    }
}
