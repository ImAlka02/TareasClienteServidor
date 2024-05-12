namespace ApiActividades.Models.DTOs
{
    public class DepartamentoDTO
    {
        public int? Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
        public int IdSuperior { get; set; }
    }
}
